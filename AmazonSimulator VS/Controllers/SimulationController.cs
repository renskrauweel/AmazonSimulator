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
            Thread.Sleep(2000); // Wait for world to be loaded, improving performance
            running = true;

            // Fetch airplane
            Airplane a = w.GetAirplanes()[0];

            // Fetch robots
            List<Robot> robots = w.GetRobots();

            PlaceAllSuitcases(robots, true);
            FetchAllSuitcases(robots);

            while (running) {
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

        private void FetchAllSuitcases(List<Robot> robots)
        {
            List<Coordinate> suitcasesCoordinates = w.GetOccupationList();
            for (int i = 0; i < suitcasesCoordinates.Count; i++)
            {
                char vertex = suitcasesCoordinates[i].GetVertex().Value;
                robots[i].AddTask(new RobotMove(g.shortest_path('A', vertex), coordinates, g));
                robots[i].AddTask(new RobotGrab(vertex, suitcasesCoordinates[i].GetSuitcase(), coordinates, g, true));
            }
        }

        private void PlaceAllSuitcases(List<Robot> robots, bool returnHome)
        {
            List<Coordinate> suitcasesCoordinates = w.GetOccupationList();
            for (int i = 0; i < suitcasesCoordinates.Count; i++)
            {
                char vertex = suitcasesCoordinates[i].GetVertex().Value;
                Robot r = robots[i];
                Suitcase s = suitcasesCoordinates[i].GetSuitcase();
                r.AddTask(new RobotGrab(vertex, s, coordinates, g, false));
                r.AddTask(new RobotMove(g.shortest_path('A', vertex), coordinates, g));
                r.AddTask(new RobotRelease(s, new Coordinate(s.x, s.y, s.z)));
                s.Move(15, 0, 5); // Move to A

                if (returnHome)
                {
                    r.AddTask(new RobotMove(g.shortest_path(vertex, 'A'), coordinates, g));
                }
            }
        }
    }
}