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
        private char? vertex { get; set; }
        private bool canBeOccupied = false;
        private Suitcase suitcase;

        /// <summary>
        /// Constructs Coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="vertex"></param>
        /// <param name="canBeOccupied"></param>
        public Coordinate(double x, double y, double z, char? vertex=null, bool canBeOccupied = false)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.vertex = vertex;
            this.canBeOccupied = canBeOccupied;
        }

        /// <summary>
        /// Get x
        /// </summary>
        /// <returns></returns>
        public double GetX()
        {
            return this.x;
        }
        /// <summary>
        /// Get y
        /// </summary>
        /// <returns></returns>
        public double GetY()
        {
            return this.y;
        }
        /// <summary>
        /// Get z
        /// </summary>
        /// <returns></returns>
        public double GetZ()
        {
            return this.z;
        }
        /// <summary>
        /// Get the vertex
        /// </summary>
        /// <returns></returns>
        public char? GetVertex()
        {
            return this.vertex;
        }
        /// <summary>
        /// Get if coordinate can be occupied or not
        /// </summary>
        /// <returns></returns>
        public bool CanBeOccupied()
        {
            return this.canBeOccupied;
        }

        /// <summary>
        /// Give a suitcase to the Coordinate
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Suitcase GiveSuitcase(Suitcase s)
        {
            this.suitcase = s;
            return this.suitcase;
        }
        /// <summary>
        /// Remove suitcase from the coordinate
        /// </summary>
        /// <returns></returns>
        public Suitcase RemoveSuitcase()
        {
            Suitcase s = this.suitcase;
            this.suitcase = null;
            return s;
        }
        /// <summary>
        /// Get suitcase from the coordinate
        /// </summary>
        /// <returns></returns>
        public Suitcase GetSuitcase()
        {
            return this.suitcase;
        }
    }
}
