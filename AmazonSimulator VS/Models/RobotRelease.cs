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

        /// <summary>
        /// Constructs the RobotRelease task
        /// </summary>
        /// <param name="suitcase"></param>
        /// <param name="home"></param>
        /// <param name="updateSuitcaseCountForTransport"></param>
        public RobotRelease(Suitcase suitcase, Coordinate home, bool updateSuitcaseCountForTransport = false)
        {
            this.suitcase = suitcase;
            this.home = home;
            this.updateSuitcaseCountForTransport = updateSuitcaseCountForTransport;
        }

        /// <summary>
        /// Start task method
        /// </summary>
        /// <param name="r"></param>
        public void StartTask(Robot r)
        {
            r.ClearSuitcase();
            
            suitcase.Move(home.GetX(), home.GetY(), home.GetZ() - 1);
        }

        /// <summary>
        /// Task complete method
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
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
