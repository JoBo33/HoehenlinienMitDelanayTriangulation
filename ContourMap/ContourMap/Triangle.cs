using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContourMap
{
    class Triangle
    {
        public Vector VertexA { get; set; }
        public Vector VertexB { get; set; }
        public Vector VertexC { get; set; }

        public Triangle()
        {

        }

        public Triangle(Vector a, Vector b, Vector c)
        {
            VertexA = a;
            VertexB = b;
            VertexC = c;
        }

        public Vector CalculateCircumCircleCentre()
        {
            StraightLine bisector1 = StraightLine.CalculatePerpendicularBisector(VertexA, VertexB);  // Mittelsenkrechte
            StraightLine bisector2 = StraightLine.CalculatePerpendicularBisector(VertexA, VertexC);

            Vector intersectionPoint = StraightLine.CalculateIntersectionPoint(bisector1, bisector2);
            return intersectionPoint;
            
        }

        public double CalculateRadius(Vector centre)
        {
            double radius = Math.Sqrt(Math.Pow(centre.X - VertexA.X, 2) + Math.Pow(centre.Y - VertexA.Y, 2));
            return radius;
        }


        public bool PointInTriangle(Vector a)
        {
            Vector cross1 = Vector.CrossProduct(VertexB - VertexA, a - VertexA);
            Vector cross2 = Vector.CrossProduct(VertexB - VertexA, VertexC - VertexA);
            int rightSideCount = 0;
            if (Vector.ScalarProduct(cross1,cross2) >= 0)
            {
                rightSideCount++;
            }
            cross1 = Vector.CrossProduct(VertexC - VertexA, a - VertexA);
            cross2 = Vector.CrossProduct(VertexC - VertexA, VertexB - VertexA);
            if (Vector.ScalarProduct(cross1, cross2) >= 0)
            {
                rightSideCount++;
            }
            cross1 = Vector.CrossProduct(VertexC - VertexB, a - VertexB);
            cross2 = Vector.CrossProduct(VertexC - VertexB, VertexA - VertexB);
            if (Vector.ScalarProduct(cross1, cross2) >= 0)
            {
                rightSideCount++;
            }

            if(rightSideCount == 3)
            {
                return true;
            }
            return false;
        }

        //public bool PointInTriangle(Vector a)
        //{
        //    double cross1 = Vector.CrossProduct2D(VertexB - VertexA, a - VertexA);
        //    double cross2 = Vector.CrossProduct2D(VertexB - VertexA, VertexC - VertexA);
        //    int rightSideCount = 0;
        //    if (cross1 * cross2 >= 0)
        //    {
        //        rightSideCount++;
        //    }
        //    cross1 = Vector.CrossProduct2D(VertexC - VertexA, a - VertexA);
        //    cross2 = Vector.CrossProduct2D(VertexC - VertexA, VertexB - VertexA);
        //    if (cross1 * cross2 >= 0)
        //    {
        //        rightSideCount++;
        //    }
        //    cross1 = Vector.CrossProduct2D(VertexC - VertexB, a - VertexB);
        //    cross2 = Vector.CrossProduct2D(VertexC - VertexB, VertexA - VertexB);
        //    if (cross1 * cross2 >= 0)
        //    {
        //        rightSideCount++;
        //    }

        //    if (rightSideCount == 3)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public static List<Triangle> BuildTriangles(List<Vector[]> edges)
        {

            List<Triangle> triangles = new List<Triangle> { };
            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < edges.Count; j++)
                {
                    if (edges[i][0] == edges[j][0] && i != j)
                    {
                        for (int k = 0; k < edges.Count; k++)
                        {
                            if (edges[i][1] == edges[k][0] && edges[j][1] == edges[k][1] || edges[i][1] == edges[k][1] && edges[j][1] == edges[k][0])
                            {
                                if (edges[i][0].X == edges[k][0].X && edges[i][0].X == edges[k][1].X ||
                                    edges[i][0].Y == edges[k][0].Y && edges[i][0].Y == edges[k][1].Y)
                                {
                                    break;
                                }
                                triangles.Add(new Triangle(edges[i][0], edges[k][0], edges[k][1]));
                                break;
                            }
                        }

                    }
                    else if (edges[i][0] == edges[j][1] && i != j)
                    {
                        for (int k = 0; k < edges.Count; k++)
                        {
                            if (edges[i][1] == edges[k][0] && edges[j][0] == edges[k][1] || edges[i][1] == edges[k][1] && edges[j][0] == edges[k][0])
                            {
                                if (edges[i][0].X == edges[k][0].X && edges[i][0].X == edges[k][1].X ||
                                    edges[i][0].Y == edges[k][0].Y && edges[i][0].Y == edges[k][1].Y)
                                {
                                    break;
                                }
                                triangles.Add(new Triangle(edges[i][0], edges[k][0], edges[k][1]));
                                break;
                            }
                        }
                    }
                }

            }
            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = 0; j < triangles.Count; j++)
                {
                    if (i != j)
                    {
                        if ((triangles[i].VertexA == triangles[j].VertexA || triangles[i].VertexA == triangles[j].VertexB || triangles[i].VertexA == triangles[j].VertexC)
                            && (triangles[i].VertexB == triangles[j].VertexA || triangles[i].VertexB == triangles[j].VertexB || triangles[i].VertexB == triangles[j].VertexC)
                            && (triangles[i].VertexC == triangles[j].VertexA || triangles[i].VertexC == triangles[j].VertexB || triangles[i].VertexC == triangles[j].VertexC))
                        {
                            triangles.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            return triangles;
        }
    }
}
