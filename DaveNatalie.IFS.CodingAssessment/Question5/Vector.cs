using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveNatalie.IFS.CodingAssessment.Question5
{
    /// <summary>
    /// Non-mutable type for storing vector information
    /// </summary>
    internal struct Vector
    {

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }



        public static Vector Origin => new Vector(0, 0);



        public Vector Add(Vector v)
        {
            return this.Add(v.X, v.Y);
        }
        public Vector Add(int x, int y)
        {
            return new Vector(this.X + x, this.Y + y);
        }

        /// <summary>
        /// Attempts to calculate the mathematical distance to another point.
        /// </summary>
        /// <param name="point">The point to measure towards</param>
        /// <returns>The calculated distance</returns>
        public int DistanceTo(Vector point)
        {
            double distance = Math.Sqrt(Math.Pow((point.X - this.X), 2) + Math.Pow((point.Y - this.Y), 2));
            return (int) Math.Round( distance);
        }

        /// <summary>
        /// Attempts to calculate the Manhattan distance to another point.
        /// </summary>
        /// <param name="point">The point to measure towards</param>
        /// <returns>The calculated distance</returns>
        public int ManhattanDistanceTo(Vector point)
        {
            int distance = Math.Abs(this.X - point.X) + Math.Abs(this.Y - point.Y);
            return distance;
       
        }


        public override string ToString()
        {
            return $"{this.X},{this.Y}";
        }
    }
}
