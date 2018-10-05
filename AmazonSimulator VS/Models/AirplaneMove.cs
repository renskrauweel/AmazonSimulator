using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class AirplaneMove : ITask<Airplane>
    {
        Coordinate destination;
        double Acc = 0;
        double Dec = 2.5;
        bool liftOff = false;
        bool Flying = false;
        //bool SpeedUp = false;
        //bool SpeedDown = false;
        //bool Go = false;

        public AirplaneMove(Coordinate destination, bool liftOff = false, bool Flying = false)
        {
            this.destination = destination;
            this.liftOff = liftOff;
            this.Flying = Flying;
        }

        public void StartTask(Airplane a)
        {
            if (!(liftOff && Flying))
            {
                a.Move(a.x + 1, a.y, a.z);
            }

            if (liftOff && !Flying)
            {
                if (a.x >= 69)
                {
                    a.Move(a.x = 70, a.y, a.z);
                }
                else
                {
                    if (Acc > 2 || Acc == 2)
                    {
                        Acc = 2;
                    }
                    else
                    {
                        Acc += 0.05;
                    }
                    a.Move(a.x + Acc, a.y, a.z);
                }
            }

            if (liftOff && Flying)
            {
                //if (a.rotationZ >= (Math.PI/6))
                //{
                //    a.Rotate(a.rotationX, a.rotationY, a.rotationZ);
                //}
                //else
                //{
                //    a.Rotate(a.rotationX, a.rotationY, a.rotationZ + (Math.PI / 20));
                //}
                a.Move(a.x + 1, a.y + .5, a.z);
            }

            if (!liftOff && Flying)
            {

                if (a.rotationY <= 0)
                {
                    a.Rotate(a.rotationX, a.rotationY, a.rotationZ);
                }
                else
                {
                    a.Rotate(a.rotationX, a.rotationY, a.rotationZ);
                }

                if (Dec < 1 || Dec == 1)
                {
                    Dec = 1;
                }
                else
                {
                    Dec -= 0.05;
                }
                a.Move(a.x + Dec, a.y - 1, a.z);
            }
        }

        public bool TaskComplete(Airplane a)
        {
            bool Complete = Math.Round(a.x, 1) == Math.Round(destination.GetX(), 1) && Math.Round(a.z, 1) == Math.Round(destination.GetZ(), 1);

            if (Complete && liftOff && Flying)
            {
                a.Move(-60, 59, -15);
            }
            //if (Complete && !(liftOff && Flying)) { }
            return Complete;
        }
    }
}