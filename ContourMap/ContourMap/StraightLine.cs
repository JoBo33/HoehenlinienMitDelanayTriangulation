using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContourMap
{
    class StraightLine
    {
        public Vector PositionVector { get; set; }
        public Vector DirectionVector { get; set; }

        public StraightLine() : this(nullVector(), nullVector())
        {}

        public StraightLine(Vector position)
        {
            PositionVector = position;
        }
        public StraightLine(Vector position,Vector direction) : this(position)
        {
            DirectionVector = direction;
        }

        private static Vector nullVector()
        {
            Vector nullVector = new Vector(0, 0, 0);
            return nullVector;
        }

        public static StraightLine EdgeToStraightLine(Vector[] edge)
        {
            StraightLine line = new StraightLine(edge[0], edge[1]-edge[0]);
            return line;
        }

        public bool PointOnLine(Vector vector)
        {
            if ((vector.X == this.PositionVector.X && this.DirectionVector.X == 0) || (vector.Y == this.PositionVector.Y && this.DirectionVector.Y == 0))
            {
                return true;
            }
            else
            {
                double firstSolution = (vector.X - this.PositionVector.X) / this.DirectionVector.X;
                double secondSolution = (vector.Y - this.PositionVector.Y) / this.DirectionVector.Y;
                if (firstSolution == secondSolution)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public static StraightLine CalculatePerpendicularBisector(Vector vertexA, Vector vertexB)
        {
            StraightLine bisector = new StraightLine();
            bisector.PositionVector = vertexA + 0.5 * (vertexB - vertexA);
            bisector.DirectionVector.X = -1 * (vertexB.Y - vertexA.Y);
            bisector.DirectionVector.Y = (vertexB.X - vertexA.X);
            return bisector;
        }

        public static Vector CalculateIntersectionPoint(StraightLine bisector1, StraightLine bisector2)
        {
            Vector intersectionPoint = new Vector(); // Schnittpunkt
            Vector help = new Vector(bisector1.DirectionVector.X, bisector1.DirectionVector.Y, bisector1.DirectionVector.Z);
            StraightLine tmp = new StraightLine((bisector1.PositionVector - bisector2.PositionVector), bisector1.DirectionVector);
            double lineVariable;
            if (bisector2.DirectionVector.X != 0 && bisector2.DirectionVector.Y != 0)
            {
                tmp.PositionVector.X /= bisector2.DirectionVector.X;
                tmp.PositionVector.Y /= bisector2.DirectionVector.Y;
                tmp.DirectionVector.X /= bisector2.DirectionVector.X;
                tmp.DirectionVector.Y /= bisector2.DirectionVector.Y;
                //if (tmp.PositionVector.Y < 0)
                //{
                //    if (tmp.DirectionVector.X < 0)
                //    {
                //        lineVariable = (tmp.PositionVector.X + tmp.PositionVector.Y) / (tmp.DirectionVector.Y + tmp.DirectionVector.X);
                //    }
                //    else
                //    {
                //        lineVariable = (tmp.PositionVector.X + tmp.PositionVector.Y) / (tmp.DirectionVector.Y - tmp.DirectionVector.X);
                //    }
                //}

                //else
                //{
                //    if (tmp.DirectionVector.X < 0)
                //    {
                //        lineVariable = (tmp.PositionVector.X - tmp.PositionVector.Y) / (tmp.DirectionVector.Y + tmp.DirectionVector.X);
                //    }
                //    else
                //    {
                //        lineVariable = (tmp.PositionVector.X - tmp.PositionVector.Y) / (tmp.DirectionVector.Y - tmp.DirectionVector.X);
                //    }
                //}
                lineVariable = (tmp.PositionVector.X - tmp.PositionVector.Y) / (tmp.DirectionVector.Y - tmp.DirectionVector.X);
                intersectionPoint = bisector1.PositionVector + lineVariable * help;
            }
            else
            {

                if (bisector2.DirectionVector.X == 0)
                {
                    lineVariable = (bisector2.PositionVector.X - bisector1.PositionVector.X) / bisector1.DirectionVector.X;
                    intersectionPoint = bisector1.PositionVector + lineVariable * help;

                }
                else if (bisector2.DirectionVector.Y == 0)
                {
                    lineVariable = (bisector1.PositionVector.Y - bisector2.PositionVector.Y) / bisector1.DirectionVector.Y;
                    intersectionPoint = bisector1.PositionVector + lineVariable * help;

                }
            }
            return intersectionPoint;
        }
    }
}
