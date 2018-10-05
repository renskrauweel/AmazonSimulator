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

        public RobotGrab(char pickupNode, Suitcase suitcase, List<Coordinate> coordinates, Graph g, bool bringHome)
        {
            this.pickupNode = pickupNode;
            this.suitcase = suitcase;
            this.coordinates = coordinates;
            this.g = g;
            this.bringHome = bringHome;
        }

        public void StartTask(Robot r)
        {
            suitcase.x = r.x;
            suitcase.y = r.y + 0.3;
            suitcase.z = r.z;

            r.SetSuitcase(suitcase);

            if (!walkingHome && bringHome)
            {
                r.AddTask(new RobotMove(g.shortest_path(pickupNode, 'A'), coordinates, g));
                r.AddTask(new RobotRelease(suitcase, home, true));
                walkingHome = true;
            }
        }

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
