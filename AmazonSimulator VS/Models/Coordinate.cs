using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Coordinate
    {
        private double x { get; set; }
        private double y { get; set; }
        private double z { get; set; }

        public Coordinate(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double GetX()
        {
            return this.x;
        }
        public double GetY()
        {
            return this.y;
        }
        public double GetZ()
        {
            return this.z;
        }
    }
}
