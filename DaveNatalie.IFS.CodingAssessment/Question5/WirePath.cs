using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveNatalie.IFS.CodingAssessment.Question5
{
    /// <summary>
    /// Represents the path a wire travels along the grid. WirePaths should be loaded from a text string.
    /// </summary>
    internal class WirePath
    {
        public ICollection<LineSegment> LineSegments { get; } = new List<LineSegment>();

        private WirePath()
        {

        }


        public static WirePath FromString(string pathString)
        {

            WirePath path = new WirePath();


            Vector cursor = Vector.Origin;

            string[] movements = pathString.Split(',');

            foreach(string movement in movements)
            {
                char direction = movement[0];
                int amount = int.Parse(movement.Substring(1));


                Vector nextPoint = movement[0] switch
                {
                    'R' => cursor.Add(amount, 0),
                    'L' => cursor.Add(-amount, 0),
                    'U' => cursor.Add(0, amount),
                    'D' => cursor.Add(0, -amount),
                    _ => throw new Exception("Movement is invalid")
                };

                LineSegment segment = new LineSegment(cursor, nextPoint);
                path.LineSegments.Add(segment);
                cursor = nextPoint;
            }

            return path;

        }

        internal void IntersectsAll(WirePath other, Action<Vector> intersectsCallback)
        {
            foreach (var segment in this.LineSegments)
            {
                foreach (var otherSegment in other.LineSegments)
                {
                    if (segment.Intersects(otherSegment, out Vector point))
                    {
                        intersectsCallback(point);
                    }
                }
            }

        }
    }
}
