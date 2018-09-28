using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : BaseModel {
        private List<IRobotTask> tasks = new List<IRobotTask>();

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

        public override bool Update(int tick)
        {
            if (tasks.Count > 0)
            {
                if (tasks.First().TaskComplete(this))
                {
                    tasks.RemoveAt(0);

                    //if (tasks.Count == 0)
                    //{
                    //    tasks = null;
                    //}

                    //tasks.First().StartTask(this);
                } else
                {
                    tasks.First().StartTask(this);
                }
            }
            //this.MoveToVertex(g.shortest_path('A', 'D'), coordinates, g);
            //this.Move(this.x + 0.01, this.y, this.z);

            return base.Update(tick);
        }

        public void AddTask(IRobotTask task)
        {
            this.tasks.Add(task);
        }
    }
}