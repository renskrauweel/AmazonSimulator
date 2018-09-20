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

        public SimulationController(World w) {
            this.w = w;
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

            List<Coordinate> coordinates = new List<Coordinate> {
                new Coordinate(15, 0, 5), // Top center - A
                new Coordinate(25, 0, 5), // Top right - E
                new Coordinate(25, 0, 10), // Right lane 1
                new Coordinate(25, 0, 15), // Right lane 2
                new Coordinate(25, 0, 20), // Right lane 3
                new Coordinate(25, 0, 25), // Bottom right - D
                new Coordinate(5, 0, 25), // Bottom left - C
                new Coordinate(5, 0, 5), // Top left - B
            };

            Node nodeA = new Node('A', 0);
            nodeA.shortestDistanceFromStart = 0;
            Node nodeB = new Node('B', 10);
            Node nodeE = new Node('E', 10);
            Node nodeD = new Node('D', 20);
            Node nodeC = new Node('C', 20);
            nodeA.nodes.AddRange(new List<Node> {nodeB, nodeE});
            nodeE.nodes.AddRange(new List<Node> { nodeA, nodeD });
            nodeD.nodes.AddRange(new List<Node> { nodeE, nodeC });
            nodeC.nodes.AddRange(new List<Node> { nodeD, nodeB });
            nodeB.nodes.AddRange(new List<Node> { nodeA, nodeC });

            // Fetch robot
            Robot r = w.GetRobots()[0];

            while (running) {
                // Move through coordinates
                foreach (Coordinate coordinate in coordinates)
                {
                    MoveToCoordinate(r, coordinate.GetX(), coordinate.GetY(), coordinate.GetZ());
                }
                // Reverse through coordinates
                for (int i = coordinates.Count - 1; i >= 0; i--)
                {
                    Coordinate coordinate = coordinates[i];
                    MoveToCoordinate(r, coordinate.GetX(), coordinate.GetY(), coordinate.GetZ());
                }

                UpdateFrame();
            }
        }

        public void EndSimulation() {
            running = false;
        }

        private void MoveToCoordinate(BaseModel bm, double coordinateX, double coordinateY, double coordinateZ)
        {
            // X
            ChangeDirection(bm, new Coordinate(coordinateX, coordinateY, coordinateZ));
            if (bm.x <= coordinateX)
            {
                LoopUpX(bm, coordinateX);
            } else
            {
                LoopDownX(bm, coordinateX);
            }
            // Y
            ChangeDirection(bm, new Coordinate(coordinateX, coordinateY, coordinateZ));
            if (bm.y <= coordinateY)
            {
                LoopUpY(bm, coordinateY);
            }
            else
            {
                LoopDownY(bm, coordinateY);
            }
            // Z
            ChangeDirection(bm, new Coordinate(coordinateX, coordinateY, coordinateZ));
            if (bm.z <= coordinateZ)
            {
                LoopUpZ(bm, coordinateZ);
            }
            else
            {
                LoopDownZ(bm, coordinateZ);
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