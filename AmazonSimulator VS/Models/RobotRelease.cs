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
        private bool updateSuitcaseCountForTransport;

        public RobotRelease(Suitcase suitcase, Coordinate home, bool updateSuitcaseCountForTransport = false)
        {
            this.suitcase = suitcase;
            this.home = home;
            this.updateSuitcaseCountForTransport = updateSuitcaseCountForTransport;
        }

        public void StartTask(Robot r)
        {
            r.ClearSuitcase();
            
            suitcase.Move(home.GetX(), home.GetY(), home.GetZ() - 1);
        }

        public bool TaskComplete(Robot r)
        {
            bool complete = (suitcase.x == home.GetX() && suitcase.y == home.GetY() && suitcase.z == home.GetZ() - 1);
            if (complete && updateSuitcaseCountForTransport)
            {
                Controllers.SimulationController.transportSuitcasesCount++;
            }
            return complete;
        }
    }
}
