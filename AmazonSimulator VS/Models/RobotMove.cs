using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RobotMove : IRobotTask
    {
        private bool startupComplete = false;
        private bool complete = false;

        List<char> verticesToTravelThrough;
        List<Coordinate> coordinates;
        Graph g;

        public RobotMove(List<char> verticesToTravelThrough, List<Coordinate> coordinates, Graph g)
        {
            this.verticesToTravelThrough = verticesToTravelThrough;
            this.coordinates = coordinates;
            this.g = g;
            GetCoordinatesToUse();
        }

        public void StartTask(Robot r)
        {
            
            r.MoveThroughCoordinates(coordinates);
        }

        public bool TaskComplete(Robot r)
        {
            return Math.Floor(r.x) == coordinates.Last().GetX() && Math.Floor(r.z) == coordinates.Last().GetZ();
        }

        private void GetCoordinatesToUse()
        {
            List<Coordinate> coordinatesToUse = new List<Coordinate>();
            for (int i = verticesToTravelThrough.Count - 1; i >= 0; i--)
            {
                foreach (Coordinate coordinate in coordinates)
                {
                    if (coordinate.GetVertex() == verticesToTravelThrough[i])
                    {
                        coordinatesToUse.Add(coordinate);
                    }
                }
            }
            this.coordinates = coordinatesToUse;
        }
    }
}
