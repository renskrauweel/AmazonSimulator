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
        public int startAt = 0;

        public SimulationController(World w) {
            this.w = w;
            this.g = w.GetGraph();

            this.coordinates = w.GetCoordinates();
        }

        public void AddView(ClientView v) {
            ObservingClient oc = new ObservingClient();

            oc.unsubscribe = this.w.Subscribe(v);
            oc.cv = v;

            views.Add(oc);
        }

        public void RemoveView(ClientView v) {
            for(int i = 0; i < views.Count; i++) {
                ObservingClient currentOC = views[i];

                if(currentOC.cv == v) {
                    views.Remove(currentOC);
                    currentOC.unsubscribe.Dispose();
                }
            }
        }

        public void Simulate() {
            Thread.Sleep(4000); // Wait for world to be loaded, improving performance
            running = true;

            // Fetch airplane
            Airplane a = w.GetAirplanes()[0];
            
            // Fetch robots
            List<Robot> robots = w.GetRobots();

            FetchAllSuitcases(robots);

            while (running) {
                if (transportSuitcasesCount == 4)
                {
                    // Move out of view
                    List<Coordinate> occupationList = w.GetOccupationList();
                    for (int i = 0; i < 4; i++)
                    {
                        Coordinate c = occupationList[i+startAt];
                        Suitcase s = c.GetSuitcase();
                        s.Move(100, 100, 100);
                    }
                    // Airplane liftoff
                    a.AddTask(new AirplaneMove(new Coordinate(70, 4.3, -15), true)); //punt 3
                    a.AddTask(new AirplaneMove(new Coordinate(125, 59, -15), true, true)); // punt 4
                    a.AddTask(new AirplaneMove(new Coordinate(-50, 4.3, -15), false, true)); //punt 1
                    a.AddTask(new AirplaneMove(new Coordinate(15, 4.3, -15))); //punt 2
                    transportSuitcasesCount = 0;
                }
                if (a.GetLanded())
                {
                    // Move suitcases to A
                    List<Coordinate> occupationList = w.GetOccupationList();
                    for (int i = 0; i < 4; i++)
                    {
                        Coordinate c = occupationList[i+startAt];
                        Suitcase s = c.GetSuitcase();
                        s.Move(15, 0, 5); // Move to A
                    }
                    PlaceAllSuitcases(robots, true, startAt);
                    a.SetLanded(false);
                    this.startAt = 4;
                }
                if (this.startAt == 4)
                {
                    FetchAllSuitcases(robots, startAt);
                    startAt = 0;
                }

                UpdateFrame();
            }
        }

        public void EndSimulation() {
            running = false;
        }

        private void UpdateFrame()
        {
            w.Update(tickTime);
            Thread.Sleep(tickTime);
        }

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