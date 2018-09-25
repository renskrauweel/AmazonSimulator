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

            coordinates = new List<Coordinate> {
                new Coordinate(15, 0, 5, 'A'), // Top center - A
                new Coordinate(25, 0, 5, 'E'), // Top right - E
                new Coordinate(25, 0, 10), // Right lane 1
                new Coordinate(25, 0, 15), // Right lane 2
                new Coordinate(25, 0, 20), // Right lane 3
                new Coordinate(25, 0, 25, 'D'), // Bottom right - D
                new Coordinate(5, 0, 25, 'C'), // Bottom left - C
                new Coordinate(5, 0, 5, 'B'), // Top left - B
            };

            g.add_vertex('A', new Dictionary<char, int>() { { 'B', 10 }, { 'E', 10 } });
            g.add_vertex('B', new Dictionary<char, int>() { { 'A', 10 }, { 'C', 20 } });
            g.add_vertex('C', new Dictionary<char, int>() { { 'B', 20 }, { 'D', 20 } });
            g.add_vertex('D', new Dictionary<char, int>() { { 'C', 20 }, { 'E', 20 } });
            g.add_vertex('E', new Dictionary<char, int>() { { 'A', 10 }, { 'D', 20 } });
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
            MoveToCoordinate(suitcases[0], new Coordinate(20, 0, 12));
            MoveToCoordinate(suitcases[1], new Coordinate(20, 0, 18));

            UpdateFrame();

            while (running) {
                // Move through vertices
                MoveToVertex(t, 'B', 'A');

                MoveToVertex(r, 'A', 'D');
                MoveToVertex(r, 'D', 'C');
                MoveToVertex(r, 'C', 'A');

                MoveToVertex(t, 'A', 'E');
                Thread.Sleep(3000); // Wait 3 seconds

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
            double degrees180 = degrees90 * 2;
            double degrees270 = degrees90 * 3;
            double degrees360 = degrees90 * 4;

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