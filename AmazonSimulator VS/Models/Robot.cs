using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Robot : BaseModel {
        private List<IRobotTask> tasks = new List<IRobotTask>();
        private Suitcase suitcase;

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
                } else
                {
                    tasks.First().StartTask(this);
                }
            }

            return base.Update(tick);
        }

        public void AddTask(IRobotTask task)
        {
            this.tasks.Add(task);
        }

        public Suitcase GetSuitcase()
        {
            return this.suitcase;
        }

        public void SetSuitcase(Suitcase s)
        {
            this.suitcase = s;
        }

        public void ClearSuitcase()
        {
            this.suitcase = null;
        }

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