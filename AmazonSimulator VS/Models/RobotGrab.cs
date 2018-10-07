using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RobotGrab : ITask<Robot>
    {
        private Suitcase suitcase;
        private char pickupNode;
        private List<Coordinate> coordinates;
        private Graph g;
        private Coordinate home = new Coordinate(15, 0, 5);
        private bool walkingHome = false;
        private bool bringHome = false;

        /// <summary>
        /// Constructs the RobotGrab task
        /// </summary>
        /// <param name="pickupNode"></param>
        /// <param name="suitcase"></param>
        /// <param name="coordinates"></param>
        /// <param name="g"></param>
        /// <param name="bringHome"></param>
        public RobotGrab(char pickupNode, Suitcase suitcase, List<Coordinate> coordinates, Graph g, bool bringHome)
        {
            this.pickupNode = pickupNode;
            this.suitcase = suitcase;
            this.coordinates = coordinates;
            this.g = g;
            this.bringHome = bringHome;
        }

        /// <summary>
        /// Start task method
        /// </summary>
        /// <param name="r"></param>
        public void StartTask(Robot r)
        {
            suitcase.x = r.x;
            suitcase.y = r.y + 0.3;
            suitcase.z = r.z;

            r.SetSuitcase(suitcase);

            if (!walkingHome && bringHome)
            {
                r.AddTask(new RobotMove(g.shortest_path(pickupNode, 'A'), coordinates, g));
                r.AddTask(new RobotRelease(suitcase, home, true, true));
                walkingHome = true;
            }
        }

        /// <summary>
        /// Task complete method
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool TaskComplete(Robot r)
        {
            bool complete = (suitcase.x == r.x && suitcase.y == r.y + 0.3 && suitcase.z == r.z);
            if (complete)
            {
                walkingHome = false;
                if (bringHome)
                {
                    suitcase.x = home.GetX();
                    suitcase.y = home.GetY();
                    suitcase.z = home.GetZ();
                }
            }
            return complete;
        }
    }
}
