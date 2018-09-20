﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Truck : BaseModel
    {
        public Truck(double x, double y, double z, double rotationX, double rotationY, double rotationZ)
        {
            this.type = "truck";
            this.guid = Guid.NewGuid();

            this.x = x;
            this.y = y;
            this.z = z;

            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
        }
    }
}
