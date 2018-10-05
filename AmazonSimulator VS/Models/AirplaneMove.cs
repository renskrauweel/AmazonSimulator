using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class AirplaneMove : ITask<Airplane>
    {
        Coordinate destination;
        double x=0;

        public AirplaneMove(Coordinate destination)
        {
            this.destination = destination;
        }

        public void StartTask(Airplane a)
        {
            if (x>2 || x==2)
            {
                x = 2;
            }
            else
            {
                x += 0.05;
            }
            a.Move(a.x + x, a.y, a.z);
        }

        public bool TaskComplete(Airplane a)
        {
            return Math.Round(a.x, 0) == Math.Round(destination.GetX(), 0) && Math.Round(a.z, 0) == Math.Round(destination.GetZ(), 0);
        }
    }
}