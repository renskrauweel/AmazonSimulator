using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Airplane : BaseModel
    {
        private List<ITask<Airplane>> tasks = new List<ITask<Airplane>>();
        private bool landed = false;

        public Airplane(double x, double y, double z, double rotationX, double rotationY, double rotationZ)
        {
            this.type = "airplane";
            this.guid = Guid.NewGuid();

            this.x = x;
            this.y = y;
            this.z = z;

            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
        }

        public override bool Update(int tick)
        {
            if (tasks.Count > 0)
            {
                if (tasks.First().TaskComplete(this))
                {
                    tasks.RemoveAt(0);
                }
                else
                {
                    tasks.First().StartTask(this);
                }
            }

            return base.Update(tick);
        }


        public void AddTask(ITask<Airplane> task)
        {
            this.tasks.Add(task);
        }

        public bool GetLanded()
        {
            return this.landed;
        }
        public void SetLanded(bool landed)
        {
            this.landed = landed;
        }

    }
}
