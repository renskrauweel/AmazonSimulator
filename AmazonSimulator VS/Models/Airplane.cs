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

        /// <summary>
        /// Constructs the airplane
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="rotationX"></param>
        /// <param name="rotationY"></param>
        /// <param name="rotationZ"></param>
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

        /// <summary>
        /// Update method for airplane
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds a task to the airplane
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(ITask<Airplane> task)
        {
            this.tasks.Add(task);
        }

        /// <summary>
        /// Get the landed property
        /// </summary>
        /// <returns></returns>
        public bool GetLanded()
        {
            return this.landed;
        }
        /// <summary>
        /// Set the landed property
        /// </summary>
        /// <param name="landed"></param>
        public void SetLanded(bool landed)
        {
            this.landed = landed;
        }

    }
}
