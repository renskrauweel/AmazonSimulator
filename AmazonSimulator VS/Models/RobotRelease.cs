using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    class RobotRelease : ITask<Robot>
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
            
            suitcase.Move(home.GetX(), home.GetY(), home.GetZ() - 1);
        }

        public bool TaskComplete(Robot r)
        {
            return (suitcase.x == home.GetX() && suitcase.y == home.GetY() && suitcase.z == home.GetZ() - 1);
        }
    }
}
