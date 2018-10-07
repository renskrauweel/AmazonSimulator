using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Models;
using Views;

namespace Controllers {
    struct ObservingClient {
        public ClientView cv;
        public IDisposable unsubscribe;
    }
    public class SimulationController {
        private World w;
        private List<ObservingClient> views = new List<ObservingClient>();
        private bool running = false;
        private int tickTime = 50;

        private Graph g = new Graph();
        private List<Coordinate> coordinates;

        public static int transportSuitcasesCount = 0;
        private int startAt = 0;
        private int timesFetchedSuitcases = 0;
        private int timesLanded = 0;

        /// <summary>
        /// Constructs SimulationController
        /// </summary>
        /// <param name="w"></param>
        public SimulationController(World w) {
            this.w = w;
            this.g = w.GetGraph();

            this.coordinates = w.GetCoordinates();
        }

        /// <summary>
        /// Adds a view
        /// </summary>
        /// <param name="v"></param>
        public void AddView(ClientView v) {
            ObservingClient oc = new ObservingClient();

            oc.unsubscribe = this.w.Subscribe(v);
            oc.cv = v;

            views.Add(oc);
        }

        /// <summary>
        /// Removes a view
        /// </summary>
        /// <param name="v"></param>
        public void RemoveView(ClientView v) {
            for(int i = 0; i < views.Count; i++) {
                ObservingClient currentOC = views[i];

                if(currentOC.cv == v) {
                    views.Remove(currentOC);
                    currentOC.unsubscribe.Dispose();
                }
            }
        }

        /// <summary>
        /// The main simulate method
        /// </summary>
        public void Simulate() {
            Thread.Sleep(4000); // Wait for world to be loaded, improving performance
            running = true;

            // Fetch airplane
            Airplane a = w.GetAirplanes()[0];
            
            // Fetch robots
            List<Robot> robots = w.GetRobots();

            // Kickstart program
            FetchAllSuitcases(robots);

            while (running) {
                if (transportSuitcasesCount == 4)
                {
                    timesFetchedSuitcases++;
                    int toAdd = 0;
                    if (timesFetchedSuitcases % 2 == 0)
                    {
                        toAdd = 4;
                    }
                    // Load into airplane
                    List<Coordinate> occupationList = w.GetOccupationList();
                    for (int i = 0; i < 4; i++)
                    {
                        Coordinate c = occupationList[i+toAdd];
                        Suitcase s = c.GetSuitcase();
                        s.Move(100, 100, 100);
                    }
                    // Airplane liftoff
                    a.AddTask(new AirplaneMove(new Coordinate(70, 4.3, -15), true)); // point 3
                    a.AddTask(new AirplaneMove(new Coordinate(125, 59, -15), true, true)); // point 4
                    a.AddTask(new AirplaneMove(new Coordinate(-50, 4.3, -15), false, true)); // point 1
                    a.AddTask(new AirplaneMove(new Coordinate(15, 4.3, -15))); // point 2
                    transportSuitcasesCount = 0;
                }
                if (a.GetLanded())
                {
                    timesLanded++;
                    // Move suitcases to A
                    List<Coordinate> occupationList = w.GetOccupationList();
                    int toAdd = 0;
                    if (timesFetchedSuitcases % 2 == 0)
                    {
                        toAdd = 4;
                    } else
                    {
                        this.startAt = 4;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Coordinate c = occupationList[i+toAdd];
                        Suitcase s = c.GetSuitcase();
                        s.Move(15, 0, 5); // Move to A
                    }
                    PlaceAllSuitcases(robots, true, toAdd);
                    a.SetLanded(false);
                    if (timesLanded == 2)
                    {
                        // Redo simulation
                        FetchAllSuitcases(robots);
                        timesLanded = 0;
                    }
                }
                if (this.startAt == 4)
                {
                    FetchAllSuitcases(robots, startAt);
                    startAt = 0;
                }

                UpdateFrame();
            }
        }

        /// <summary>
        /// Ends simulation
        /// </summary>
        public void EndSimulation() {
            running = false;
        }

        /// <summary>
        /// Updates frame by ticktime
        /// </summary>
        private void UpdateFrame()
        {
            w.Update(tickTime);
            Thread.Sleep(tickTime);
        }

        /// <summary>
        /// Fetch the suitcases
        /// </summary>
        /// <param name="robots"></param>
        /// <param name="startAt"></param>
        private void FetchAllSuitcases(List<Robot> robots, int startAt = 0)
        {
            List<Coordinate> suitcasesCoordinates = w.GetOccupationList();
            for (int i = 0; i < robots.Count; i++)
            {
                int j = i + startAt;
                char vertex = suitcasesCoordinates[j].GetVertex().Value;
                robots[i].AddTask(new RobotMove(g.shortest_path('A', vertex), coordinates, g));
                robots[i].AddTask(new RobotGrab(vertex, suitcasesCoordinates[j].GetSuitcase(), coordinates, g, true));
            }
        }

        /// <summary>
        /// Places the suitcases
        /// </summary>
        /// <param name="robots"></param>
        /// <param name="returnHome"></param>
        /// <param name="startAt"></param>
        private void PlaceAllSuitcases(List<Robot> robots, bool returnHome, int startAt = 0)
        {
            List<Coordinate> suitcasesCoordinates = w.GetOccupationList();
            for (int i = 0; i < robots.Count; i++)
            {
                int j = i + startAt;
                char vertex = suitcasesCoordinates[j].GetVertex().Value;
                Robot r = robots[i];
                Suitcase s = suitcasesCoordinates[j].GetSuitcase();
                Coordinate destination = suitcasesCoordinates[j];
                r.AddTask(new RobotGrab(vertex, s, coordinates, g, false));
                r.AddTask(new RobotMove(g.shortest_path('A', vertex), coordinates, g));
                r.AddTask(new RobotRelease(s, new Coordinate(destination.GetX(), destination.GetY(), destination.GetZ())));
                s.Move(15, 0, 5); // Move to A

                if (returnHome)
                {
                    r.AddTask(new RobotMove(g.shortest_path(vertex, 'A'), coordinates, g));
                }
            }
        }
    }
}