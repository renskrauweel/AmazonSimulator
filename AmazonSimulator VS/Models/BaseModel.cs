using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public abstract class BaseModel : IUpdatable
    {
        /*protected double _x = 0;
        protected double _y = 0;
        protected double _z = 0;
        protected double _rX = 0;
        protected double _rY = 0;
        protected double _rZ = 0;*/

        public string type { get; set; }
        public Guid guid { get; set; }
        public double x = 0;
        public double y = 0;
        public double z = 0;
        public double rotationX = 0;
        public double rotationY = 0;
        public double rotationZ = 0;

        public bool needsUpdate = true;

        public virtual void Move(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            needsUpdate = true;
        }

        public virtual void Rotate(double rotationX, double rotationY, double rotationZ)
        {
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;

            needsUpdate = true;
        }

        public virtual bool Update(int tick)
        {
            if (needsUpdate)
            {
                needsUpdate = false;
                return true;
            }
            return false;
        }
    }
}
