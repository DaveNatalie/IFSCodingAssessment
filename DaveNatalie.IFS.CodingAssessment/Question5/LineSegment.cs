using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveNatalie.IFS.CodingAssessment.Question5
{
    /// <summary>
    /// A line segment represents the shortest distance between two points.
    /// </summary>
    internal class LineSegment
    {
        public LineSegment(Vector pointA, Vector pointB)
        {
            PointA = pointA;
            PointB = pointB;


            //Min and Max are eagerly calculated here because they are used frequently
            Min = new Vector(Math.Min(pointA.X, pointB.X), Math.Min(pointA.Y, pointB.Y));
            Max = new Vector(Math.Max(pointA.X, pointB.X), Math.Max(pointA.Y, pointB.Y));

#if DEBUG
            if (Min.X == 0 && Max.X == 0 && Min.Y == 0 && Max.Y == 0)
            {
                throw new InvalidOperationException("There is a problem with the points of the line segment");
            }
#endif
        }

        public Vector PointA { get; }
        public Vector PointB { get; }



        public Vector Min { get; }
        public Vector Max { get; }


        /// <summary>
        /// A line is considered vertical when the X values of each point are identical. 
        /// For the purposes of this application, any non vertical line can be considered horizontal
        /// </summary>
        public bool IsVertical => this.PointA.X == this.PointB.X;



        /// <summary>
        /// Determines if this line segments intersects with another line segments. Only works if lines are horizontal and vertical.
        /// </summary>
        /// <param name="other">Line segment to test intersection of</param>
        /// <param name="point">If intersecting, will be set to the point of intersection.</param>
        /// <returns></returns>
        internal bool Intersects(LineSegment other, out Vector point)
        {
            //TODO There is probably a more elegant solution for detecting the intersection between LineSegments.
            //My attempts to use a more mathematical approach were unsuccessful

            if (!this.IsVertical && other.IsVertical)
            {

                if ((other.PointA.X > this.Min.X && other.PointA.X < this.Max.X)
                    && (other.Max.Y > this.PointA.Y && other.Min.Y < this.PointA.Y))
                {
                    point = new Vector(other.PointA.X, this.PointA.Y);
                    return true;
                }
            }
            else if (this.IsVertical && !other.IsVertical)
            {
                if ((other.PointA.Y > this.Min.Y && other.PointA.Y < this.Max.Y)
                    && (other.Max.X > this.PointA.X && other.Min.X < this.PointA.X))
                {
                    point = new Vector(this.PointA.X, other.PointA.Y);
                    return true;
                }
            }

            point = Vector.Origin;
            return false;
        }


        public override string ToString()
        {
            return $"({this.PointA})({this.PointB})";
        }

    }
}
