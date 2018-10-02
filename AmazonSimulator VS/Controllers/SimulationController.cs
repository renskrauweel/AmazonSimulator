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

            this.coordinates = w.GetCoordinates();
            
            /*g.add_vertex('A', new Dictionary<char, int>() { { 'B', 10 }, { 'E', 10 } });
            g.add_vertex('B', new Dictionary<char, int>() { { 'A', 10 }, { 'C', 20 } });
            g.add_vertex('C', new Dictionary<char, int>() { { 'B', 20 }, { 'D', 20 } });
            g.add_vertex('D', new Dictionary<char, int>() { { 'C', 20 }, { 'E', 20 } });
            g.add_vertex('E', new Dictionary<char, int>() { { 'A', 10 }, { 'D', 20 } });*/
            
            
            //Start
            g.add_vertex('A', new Dictionary<char, int>() { { 'B', 10 }, { 'K', 10 } });

            //Right Lane
            g.add_vertex('B', new Dictionary<char, int>() { { 'A', 10 }, { 'C', 20 } });
            g.add_vertex('C', new Dictionary<char, int>() { { 'B', 20 }, { 'D', 30 }, { 'N', 30 } });
            g.add_vertex('D', new Dictionary<char, int>() { { 'C', 30 }, { 'E', 40 }, { 'T', 40 } });
            g.add_vertex('E', new Dictionary<char, int>() { { 'D', 40 }, { 'F', 50 }, { 'Z', 50 } });
            g.add_vertex('F', new Dictionary<char, int>() { { 'E', 50 }, { 'G', 70 } });

            //Left Lane
            g.add_vertex('K', new Dictionary<char, int>() { { 'A', 10 }, { 'J', 20 } });
            g.add_vertex('J', new Dictionary<char, int>() { { 'K', 20 }, { 'I', 30 }, { 'L', 30 } });
            g.add_vertex('I', new Dictionary<char, int>() { { 'J', 30 }, { 'H', 40 }, { 'R', 40 } });
            g.add_vertex('H', new Dictionary<char, int>() { { 'I', 40 }, { 'G', 50 }, { 'X', 50 } });
            g.add_vertex('G', new Dictionary<char, int>() { { 'H', 50 }, { 'F', 70 } });

            //First Inner Sector
            g.add_vertex('L', new Dictionary<char, int>() { { 'J', 30 }, { 'M', 50 }, { 'Q', 40 } });
            g.add_vertex('M', new Dictionary<char, int>() { { 'N', 40 }, { 'L', 50 }, { 'P', 50 } });
            g.add_vertex('N', new Dictionary<char, int>() { { 'C', 30 }, { 'M', 40 }, { 'O', 40 } });
            g.add_vertex('O', new Dictionary<char, int>() { { 'N', 40 } });
            g.add_vertex('P', new Dictionary<char, int>() { { 'M', 50 } });
            g.add_vertex('Q', new Dictionary<char, int>() { { 'L', 40 } });

            //Second Inner Sector
            g.add_vertex('R', new Dictionary<char, int>() { { 'I', 40 }, { 'S', 60 }, { 'W', 50 } });
            g.add_vertex('S', new Dictionary<char, int>() { { 'T', 50 }, { 'R', 60 }, { 'V', 60 } });
            g.add_vertex('T', new Dictionary<char, int>() { { 'D', 40 }, { 'S', 50 }, { 'U', 50 } });
            g.add_vertex('U', new Dictionary<char, int>() { { 'T', 50 } });
            g.add_vertex('V', new Dictionary<char, int>() { { 'S', 60 } });
            g.add_vertex('W', new Dictionary<char, int>() { { 'R', 50 } });

            //Third Inner Sector
            g.add_vertex('X', new Dictionary<char, int>() { { 'H', 50 }, { 'Y', 70 }, { '3', 60 } });
            g.add_vertex('Y', new Dictionary<char, int>() { { 'Z', 60 }, { 'X', 70 }, { '2', 70 } });
            g.add_vertex('Z', new Dictionary<char, int>() { { 'E', 50 }, { 'Y', 60 }, { '1', 60 } });
            g.add_vertex('1', new Dictionary<char, int>() { { 'Z', 60 } });
            g.add_vertex('2', new Dictionary<char, int>() { { 'Y', 70 } });
            g.add_vertex('3', new Dictionary<char, int>() { { 'X', 60 } });
            
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
            running = true;

            // Fetch truck
            Truck t = w.GetTrucks()[0];

            // Fetch robot
            Robot r = w.GetRobots()[0];
            r.Move(15, 0, 5); // Move to A

            List<Suitcase> suitcases = w.GetSuitcases();
            //s.Move(25, 0, 10);
            //MoveToCoordinate(suitcases[0], new Coordinate(20, 0, 12));
            //MoveToCoordinate(suitcases[1], new Coordinate(20, 0, 18));

            r.AddTask(new RobotMove(g.shortest_path('A', 'D'), coordinates, g));
            r.AddTask(new RobotMove(g.shortest_path('D', 'B'), coordinates, g));
            while (running) {
                UpdateFrame();
            }
        }

        public void EndSimulation() {
            running = false;
        }

        private void MoveToCoordinate(BaseModel bm, Coordinate coordinate)
        {
            double coordinateX = coordinate.GetX();
            double coordinateY = coordinate.GetY();
            double coordinateZ = coordinate.GetZ();

            // X
            ChangeDirection(bm, coordinate);
            if (bm.x <= coordinateX)
            {
                LoopUpX(bm, coordinateX);
            } else
            {
                LoopDownX(bm, coordinateX);
            }
            // Y
            ChangeDirection(bm, coordinate);
            if (bm.y <= coordinateY)
            {
                LoopUpY(bm, coordinateY);
            }
            else
            {
                LoopDownY(bm, coordinateY);
            }
            // Z
            ChangeDirection(bm, coordinate);
            if (bm.z <= coordinateZ)
            {
                LoopUpZ(bm, coordinateZ);
            }
            else
            {
                LoopDownZ(bm, coordinateZ);
            }
        }

        private void MoveToVertex(BaseModel bm, char vertexStart, char vertexTo)
        {
            List<char> verticesToTravelThrough = g.shortest_path(vertexStart, vertexTo);
            for (int i = verticesToTravelThrough.Count-1; i >= 0; i--)
            {
                foreach (Coordinate coordinate in this.coordinates)
                {
                    if (coordinate.GetVertex() == verticesToTravelThrough[i])
                    {
                        MoveToCoordinate(bm, coordinate);
                    }
                }
            }
        }

        private void UpdateFrame()
        {
            w.Update(tickTime);
            Thread.Sleep(tickTime);
        }

        // Movement loop methods
        private void LoopDownX(BaseModel bm, double coordinateX)
        {
            for (double x = bm.x; x >= coordinateX; x -= 0.2)
            {
                bm.Move(x, bm.y, bm.z);
                UpdateFrame();
            }
        }
        private void LoopUpX(BaseModel bm, double coordinateX)
        {
            for (double x = bm.x; x <= coordinateX; x += 0.2)
            {
                bm.Move(x, bm.y, bm.z);
                UpdateFrame();
            }
        }
        private void LoopDownY(BaseModel bm, double coordinateY)
        {
            for (double y = bm.y; y >= coordinateY; y -= 0.2)
            {
                bm.Move(bm.x, y, bm.z);
                UpdateFrame();
            }
        }
        private void LoopUpY(BaseModel bm, double coordinateY)
        {
            for (double y = bm.y; y <= coordinateY; y += 0.2)
            {
                bm.Move(bm.x, y, bm.z);
                UpdateFrame();
            }
        }
        private void LoopDownZ(BaseModel bm, double coordinateZ)
        {
            for (double z = bm.z; z >= coordinateZ; z -= 0.2)
            {
                bm.Move(bm.x, bm.y, z);
                UpdateFrame();
            }
        }
        private void LoopUpZ(BaseModel bm, double coordinateZ)
        {
            for (double z = bm.z; z <= coordinateZ; z += 0.2)
            {
                bm.Move(bm.x, bm.y, z);
                UpdateFrame();
            }
        }

        private void ChangeDirection(BaseModel bm, Coordinate coordinateToGo)
        {
            double degrees90 = Math.PI / 2;
            double degrees180 = Math.PI;
            double degrees270 = Math.PI * 1.5;
            double degrees360 = Math.PI * 2;

            double currentRotationY = bm.rotationY;

            // Move right
            if (coordinateToGo.GetX() > bm.x && currentRotationY != degrees90)
            {
                bm.Rotate(0, degrees90, 0);
            }
            // Move left
            if (coordinateToGo.GetX() < bm.x && currentRotationY != degrees180)
            {
                bm.Rotate(0, degrees180, 0);
            }
            // Move down
            if (coordinateToGo.GetZ() > bm.z && currentRotationY != degrees270)
            {
                bm.Rotate(0, degrees180, 0);
            }
            // Move up
            if (coordinateToGo.GetZ() < bm.z && currentRotationY != degrees360)
            {
                bm.Rotate(0, degrees360, 0);
            }
        }
    }
}