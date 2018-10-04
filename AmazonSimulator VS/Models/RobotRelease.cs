using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    class RobotRelease : IRobotTask
    {
        private Suitcase suitcase;
        private Coordinate home;

        public RobotRelease(Suitcase suitcase, Coordinate home)
        {
            this.suitcase = suitcase;
            this.home = home;
        }

        public void StartTask(Robot r)
        {
            r.ClearSuitcase();

            suitcase.x = home.GetX();
            suitcase.y = home.GetY();
            suitcase.z = home.GetZ() - 1;
        }

        public bool TaskComplete(Robot r)
        {
            return (suitcase.x == home.GetX() && suitcase.y == home.GetY() && suitcase.z == home.GetZ() - 1);
        }
    }
}
