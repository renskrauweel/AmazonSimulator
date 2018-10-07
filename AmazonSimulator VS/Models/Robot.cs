using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : BaseModel {
        private List<ITask<Robot>> tasks = new List<ITask<Robot>>();
        private Suitcase suitcase;

        /// <summary>
        /// Constructs the robot
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="rotationX"></param>
        /// <param name="rotationY"></param>
        /// <param name="rotationZ"></param>
        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) {
            this.type = "robot";
            this.guid = Guid.NewGuid();

            this.x = x;
            this.y = y;
            this.z = z;

            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
        }

        /// <summary>
        /// Update method
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
                } else
                {
                    tasks.First().StartTask(this);
                }
            }

            return base.Update(tick);
        }

        /// <summary>
        /// Add task to robot
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(ITask<Robot> task)
        {
            this.tasks.Add(task);
        }

        /// <summary>
        /// Get robot's suitcase
        /// </summary>
        /// <returns></returns>
        public Suitcase GetSuitcase()
        {
            return this.suitcase;
        }

        /// <summary>
        /// Set suitcase to robot
        /// </summary>
        /// <param name="s"></param>
        public void SetSuitcase(Suitcase s)
        {
            this.suitcase = s;
        }

        /// <summary>
        /// Clear suitcase from robot
        /// </summary>
        public void ClearSuitcase()
        {
            this.suitcase = null;
        }

        /// <summary>
        /// Move the robot, also it's suitcase if present
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public override void Move(double x, double y, double z)
        {
            base.Move(x, y, z);

            if (this.suitcase != null)
            {
                this.suitcase.Move(x, y+0.3, z);
            }
        }
    }
}