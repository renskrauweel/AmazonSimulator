using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RobotMove : ITask<Robot>
    {
        List<char> verticesToTravelThrough;
        List<Coordinate> coordinates;
        Coordinate destination;
        Graph g;

        /// <summary>
        /// Constructs the RobotMove task
        /// </summary>
        /// <param name="verticesToTravelThrough"></param>
        /// <param name="coordinates"></param>
        /// <param name="g"></param>
        public RobotMove(List<char> verticesToTravelThrough, List<Coordinate> coordinates, Graph g)
        {
            this.verticesToTravelThrough = verticesToTravelThrough;
            this.coordinates = coordinates;
            this.g = g;
            GetCoordinatesToUse();
        }

        /// <summary>
        /// Start task method
        /// </summary>
        /// <param name="r"></param>
        public void StartTask(Robot r)
        {
            r.MoveThroughCoordinates(coordinates);
        }

        /// <summary>
        /// Task complete method
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool TaskComplete(Robot r)
        {
            return Math.Round(r.x, 1) == Math.Round(destination.GetX(), 1) && Math.Round(r.z, 1) == Math.Round(destination.GetZ(), 1);
        }

        /// <summary>
        /// Gets the coordinates the robot has to use
        /// </summary>
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
            this.destination = coordinatesToUse.Last();
        }
    }
}
