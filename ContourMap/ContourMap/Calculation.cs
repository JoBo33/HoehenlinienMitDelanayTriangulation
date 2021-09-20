using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContourMap
{
    class Calculation
    {
        public static void DeterminePointsInOneRow(List<double[]> data, ref int pointsInOneRow)
        {
            pointsInOneRow = 0;
            for (int i = 0; i < data.Count; i += (1))
            {
                pointsInOneRow = pointsInOneRow + 1;
                if (data[i][1] != data[i + 1][1])
                {
                    break;
                }
            }
        }
        public static double CalculateVolume(List<double[]> data, int pointsInOneRow)
        {
            double volume = 0;
            double ground = Math.Pow((data[1][0] - data[0][0]), 2);

            for (int i = 0; i < data.Count - pointsInOneRow - 1; i += (1))
            {
                if ((i % pointsInOneRow) - (pointsInOneRow - 1) == 0 && i != 0)
                {
                    continue;
                }

                double height = (data[i][2] + data[i + 1][2] + data[i + pointsInOneRow][2] + data[i + pointsInOneRow + 1][2]) / 4;
                volume = volume + ground * height;
            }
            return volume;
        }

        public static double CalculateVolume(List<Triangle> triangles)
        {
            double volume = 0;
            for (int i = 0; i < triangles.Count; i++)
            {
                Vector direction1 = new Vector();
                direction1 = triangles[i].VertexB - triangles[i].VertexA;
                Vector direction2 = new Vector(-direction1.Y , direction1.X,direction1.Z);
                StraightLine triangleEdge = new StraightLine(triangles[i].VertexA, direction1);
                StraightLine triangleHeightLine = new StraightLine(triangles[i].VertexC, direction2);

                Vector intersectionPoint = StraightLine.CalculateIntersectionPoint(triangleEdge, triangleHeightLine);

                double triangleHeight = Vector.CalculateDistance(intersectionPoint, triangles[i].VertexC);
                double edgeLength = Vector.CalculateDistance(triangles[i].VertexA, triangles[i].VertexB);

                double ground = 0.5 * edgeLength * triangleHeight;

                double trianglePrismHeight = (triangles[i].VertexA.Z + triangles[i].VertexB.Z + triangles[i].VertexC.Z) / 3;

                volume += ground * trianglePrismHeight;

            }
            return volume;
        }

        public static double CalculateTimeTrucksNeeded(double volume, int i)
        {
            return Math.Ceiling((volume / 7) / i) * 30;
        }

        public static void DetermineContourLines(List<double[]> data, PlotModel model, int pointsInOneRow, double maxHeight)
        {
            LineSeries gridPoints = new LineSeries()
            {
                MarkerType = MarkerType.Circle,
                LineStyle = LineStyle.None,
                Color = OxyColors.Black
            };
            foreach (double[] point in data)
            {

                gridPoints.Points.Add(new DataPoint(point[0], point[1]));
            }

            model.Series.Add(gridPoints);

            for (int i = 0; i <= maxHeight; i += (1))
            {
                List<double[]> contourLine = new List<double[]> { };

                for (int j = 0; j < data.Count - 1; j += (1))
                {
                    if (j < data.Count - pointsInOneRow)
                    {
                        if (data[j][2] < i && data[j + pointsInOneRow][2] > i || data[j][2] > i && data[j + pointsInOneRow][2] < i)
                        {
                            double tmp = (i - data[j][2]) / (data[j + pointsInOneRow][2] - data[j][2]);
                            double[] pointOnHeight = { data[j][0] + tmp * (data[j + pointsInOneRow][0] - data[j][0]), data[j][1] + tmp * (data[j + pointsInOneRow][1] - data[j][1]), i };
                            contourLine.Add(pointOnHeight);
                            LineSeries points = new LineSeries()
                            {
                                MarkerType = MarkerType.Circle,
                                LineStyle = LineStyle.None,
                                Color = OxyColors.Red
                            };
                            points.Points.Add(new DataPoint(pointOnHeight[0], pointOnHeight[1]));

                            model.Series.Add(points);
                        }
                    }
                    if ((j % pointsInOneRow) - (pointsInOneRow - 1) == 0 && j != 0)
                    {
                        continue;
                    }
                    if (data[j][2] < i && data[j + 1][2] > i || data[j][2] > i && data[j + 1][2] < i)
                    {
                        double tmp = (i - data[j][2]) / (data[j + 1][2] - data[j][2]);
                        double[] pointOnHeight = { data[j][0] + tmp * (data[j + 1][0] - data[j][0]), data[j][1] + tmp * (data[j + 1][1] - data[j][1]), i };
                        contourLine.Add(pointOnHeight);
                        LineSeries points = new LineSeries()
                        {
                            MarkerType = MarkerType.Circle,
                            LineStyle = LineStyle.None,
                            Color = OxyColors.Red
                        };
                        points.Points.Add(new DataPoint(pointOnHeight[0], pointOnHeight[1]));

                        model.Series.Add(points);
                    }
                }
                //if (data[data.Count - 1][2] == i)
                //{
                //    contourLine.Add(data[data.Count - 1]);
                //}
                while (contourLine.Count > 2)
                {
                    List<double[]> oneContourLine = Drawing.SortContourPoints(ref contourLine);
                    Drawing.CatmullRomSplines(oneContourLine, model);
                }
            }
        }

       

        public static void DetermineContourLines(List<Vector[]> edges, PlotModel model, double maxHeight)
        {
            for (int i = 0; i <= maxHeight; i += (1))
            {
                List<double[]> contourLine = new List<double[]> { };

                for (int j = 0; j < edges.Count; j += (1))
                {
                    if (edges[j][0].Z > i && edges[j][1].Z < i || edges[j][0].Z < i && edges[j][1].Z > i)
                    {
                        double tmp = (i - edges[j][0].Z) / (edges[j][1].Z - edges[j][0].Z);
                        double[] pointOnHeight = { edges[j][0].X + tmp* (edges[j][1].X - edges[j][0].X) , edges[j][0].Y + tmp * (edges[j][1].Y - edges[j][0].Y), i};
                        contourLine.Add(pointOnHeight);
                        LineSeries points = new LineSeries()
                        {
                            MarkerType = MarkerType.Circle,
                            LineStyle = LineStyle.None,
                            Color = OxyColors.Red
                        };
                        points.Points.Add(new DataPoint(pointOnHeight[0], pointOnHeight[1]));

                        model.Series.Add(points);
                    }


                }
                while (contourLine.Count > 2)
                {
                    List<double[]> oneContourLine = Drawing.SortContourPoints(ref contourLine);
                    Drawing.CatmullRomSplines(oneContourLine, model);
                }
            }
        }


        public static List<Vector[]> DelaunayTriangulation(List<double[]> data)
        {
            List<Vector> vData = new List<Vector> { };
            for (int i = 0; i < data.Count; i++)
            {
                Vector point = new Vector(data[i][0], data[i][1], data[i][2]);
                vData.Add(point);
            }
            List<Vector[]> edges = new List<Vector[]> { };
            Triangle superTriangle = BuildSuperTriangle(vData);
            List<Triangle> triangles = new List<Triangle> { superTriangle };


            Vector[] superEdge1 = new Vector[2];
            superEdge1[0] = superTriangle.VertexA;
            superEdge1[1] = superTriangle.VertexB;
            edges.Add(superEdge1);

            Vector[] superEdge2 = new Vector[2];
            superEdge2[0] = superTriangle.VertexB;
            superEdge2[1] = superTriangle.VertexC;
            edges.Add(superEdge2);

            Vector[] superEdge3 = new Vector[2];
            superEdge3[0] = superTriangle.VertexC;
            superEdge3[1] = superTriangle.VertexA;
            edges.Add(superEdge3);

            Vector[] edge1Point1 = new Vector[2];
            edge1Point1[0] = vData[0];
            edge1Point1[1] = superTriangle.VertexA;
            edges.Add(edge1Point1);

            Vector[] edge2Point1 = new Vector[2];
            edge2Point1[0] = vData[0];
            edge2Point1[1] = superTriangle.VertexB;
            edges.Add(edge2Point1);

            Vector[] edge3Point1 = new Vector[2];
            edge3Point1[0] = vData[0];
            edge3Point1[1] = superTriangle.VertexC;
            edges.Add(edge3Point1);


            for (int j = 1; j < vData.Count; j++)
            {
                List<Triangle> suspectTriangle = new List<Triangle> { };
                triangles = Triangle.BuildTriangles(edges);

                //break;
                for (int i = 0; i < triangles.Count; i++)
                {

                    Vector centre = triangles[i].CalculateCircumCircleCentre();
                    double radius = triangles[i].CalculateRadius(centre);
                    DetectSuspectTriangles(ref suspectTriangle, radius, centre, vData[j], triangles[i]);
                }
                DeleteFalseTriangles(edges, suspectTriangle);
                DeleteAndAddEdges(ref edges, suspectTriangle, vData[j]);
            }
            DeleteSuperTriangleConnections(ref edges, superTriangle);
            triangles = Triangle.BuildTriangles(edges);
            //Drawing.DrawEdges(edges, model);
            return edges;
        }

        private static void DetectSuspectTriangles(ref List<Triangle> suspectTriangle, double radius, Vector centre, Vector currentPoint,Triangle currentTriangle)
        {
            double distance = Vector.CalculateDistance(centre, currentPoint); // Math.Sqrt(Math.Pow(currentPoint.X - centre.X, 2) + Math.Pow(currentPoint.Y - centre.Y, 2));

            if (radius > distance)
            {
                suspectTriangle.Add(currentTriangle);
            }
        }

        private static void DeleteFalseTriangles(List<Vector[]> edges, List<Triangle> suspectTriangle)
        {
            for (int i = 1; i < suspectTriangle.Count; i++)
            {
                for (int k = 1; k < suspectTriangle.Count; k++)
                {
                    if (i == k)
                    {
                        continue;
                    }
                    Vector vertex1 = new Vector();
                    Vector vertex2 = new Vector();
                    if (Vector.Equals(suspectTriangle[i].VertexA, suspectTriangle[k].VertexA))
                    {
                        vertex1 = suspectTriangle[k].VertexA;
                    }
                    else if (Vector.Equals(suspectTriangle[i].VertexA, suspectTriangle[k].VertexB))
                    {
                        vertex1 = suspectTriangle[k].VertexB;
                    }
                    else if (Vector.Equals(suspectTriangle[i].VertexA, suspectTriangle[k].VertexC))
                    {
                        vertex1 = suspectTriangle[k].VertexC;
                    }
                    if (Vector.Equals(suspectTriangle[i].VertexB, suspectTriangle[k].VertexA))
                    {
                        if (Vector.Equals(vertex1, new Vector(0, 0, 0)))
                        {
                            vertex2 = suspectTriangle[k].VertexA;
                        }
                        else
                        {
                            vertex1 = suspectTriangle[k].VertexA;
                        }
                    }
                    else if (Vector.Equals(suspectTriangle[i].VertexB, suspectTriangle[k].VertexB))
                    {
                        if (Vector.Equals(vertex1, new Vector(0, 0, 0)))
                        {
                            vertex2 = suspectTriangle[k].VertexB;
                        }
                        else
                        {
                            vertex1 = suspectTriangle[k].VertexB;
                        }
                    }
                    else if (Vector.Equals(suspectTriangle[i].VertexB, suspectTriangle[k].VertexC))
                    {
                        if (Vector.Equals(vertex1, new Vector(0, 0, 0)))
                        {
                            vertex2 = suspectTriangle[k].VertexC;
                        }
                        else
                        {
                            vertex1 = suspectTriangle[k].VertexC;
                        }
                    }
                    if (suspectTriangle[i].VertexC == suspectTriangle[k].VertexA)
                    {
                        vertex2 = suspectTriangle[k].VertexA;
                    }
                    else if (suspectTriangle[i].VertexC == suspectTriangle[k].VertexB)
                    {
                        vertex2 = suspectTriangle[k].VertexB;
                    }
                    else if (suspectTriangle[i].VertexC == suspectTriangle[k].VertexC)
                    {
                        vertex2 = suspectTriangle[k].VertexC;
                    }

                    if (!Vector.Equals(vertex1, new Vector(0, 0, 0)) && !Vector.Equals(vertex2, new Vector(0, 0, 0)))
                    {
                        if (!Vector.Equals(suspectTriangle[k].VertexA, vertex1) && !Vector.Equals(suspectTriangle[k].VertexA, vertex2))
                        {
                            if (suspectTriangle[i].PointInTriangle(suspectTriangle[k].VertexA))
                            {
                                suspectTriangle.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                        else if (!Vector.Equals(suspectTriangle[k].VertexB, vertex1) && !Vector.Equals(suspectTriangle[k].VertexB, vertex2))
                        {
                            if (suspectTriangle[i].PointInTriangle(suspectTriangle[k].VertexB))
                            {
                                suspectTriangle.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                        else if (!Vector.Equals(suspectTriangle[k].VertexC, vertex1) && !Vector.Equals(suspectTriangle[k].VertexC, vertex2))
                        {
                            if (suspectTriangle[i].PointInTriangle(suspectTriangle[k].VertexC))
                            {
                                suspectTriangle.RemoveAt(i);
                                i--;
                                break;
                            }
                        }

                    }
                }
            }
        }

        private static void DeleteSuperTriangleConnections(ref List<Vector[]> edges, Triangle superTriangle)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (Vector.Equals(edges[i][0], superTriangle.VertexA) || Vector.Equals(edges[i][1], superTriangle.VertexA)
                    || Vector.Equals(edges[i][0], superTriangle.VertexB) || Vector.Equals(edges[i][1], superTriangle.VertexB)
                    || Vector.Equals(edges[i][0], superTriangle.VertexC) || Vector.Equals(edges[i][1], superTriangle.VertexC))
                {
                    edges.RemoveAt(i);
                    i--;
                }
            }
        }

        public static double CalculateHeight(Vector intersectionPoint, StraightLine currentEdgeLine)
        {
            double variable = 0;
            if(currentEdgeLine.DirectionVector.X != 0)
            {
                //variable is in this case the solution of the first equation of the linear system of equations 
                variable = (intersectionPoint.X - currentEdgeLine.PositionVector.X) / currentEdgeLine.DirectionVector.X; 
            }
            else
            {
                //variable is in this case the solution of the second equation of the linear system of equations 
                variable = (intersectionPoint.Y - currentEdgeLine.PositionVector.Y) / currentEdgeLine.DirectionVector.Y;
            }
            // insert the variable in the third equation and converse it to the searched height
            double searchedHeight = variable * currentEdgeLine.DirectionVector.Z - currentEdgeLine.PositionVector.Z;
            return searchedHeight;

        }

        private static void DeleteAndAddEdges(ref List<Vector[]> edges, List<Triangle> suspectTriangle, Vector currentPoint)
        {
            int numberOfEdges = edges.Count;
            //Vector vector1 = new Vector(edges[18][0].X, edges[18][0].Y, edges[18][0].Z);
            //Vector vector2 = new Vector(edges[18][1].X, edges[18][1].Y, edges[18][1].Z);
            //Vector[] vec = new Vector[2] { vector1, vector2 };
            for (int i = 0; i < numberOfEdges; i++)
            {
                Triangle tmp1 = new Triangle();
                Triangle tmp2 = new Triangle();
                int containsCurrentEdge = 0;
                for (int j = 1; j < suspectTriangle.Count; j++)
                {
                    if (i > edges.Count-1)
                    {
                        return;
                    }
                    if ((edges[i][0] == suspectTriangle[j].VertexA || edges[i][0] == suspectTriangle[j].VertexB || edges[i][0] == suspectTriangle[j].VertexC) // Contain a tri the edge?
                        && (edges[i][1] == suspectTriangle[j].VertexA || edges[i][1] == suspectTriangle[j].VertexB || edges[i][1] == suspectTriangle[j].VertexC))
                    {
                        containsCurrentEdge++;
                        if (tmp1.VertexA == null)
                        {
                            tmp1 = new Triangle(suspectTriangle[j].VertexA, suspectTriangle[j].VertexB, suspectTriangle[j].VertexC);
                        }
                        else
                        {
                            tmp2 = new Triangle(suspectTriangle[j].VertexA, suspectTriangle[j].VertexB, suspectTriangle[j].VertexC);
                        }
                    }

                }
                if (containsCurrentEdge >= 1)
                {
                    if (containsCurrentEdge == 2 && i > 2)
                    {
                        edges.RemoveAt(i);
                        i--;
                    }
                    if (!(tmp1.VertexA == null))
                    {
                        bool edgeAExist = false;
                        bool edgeBExist = false;
                        bool edgeCExist = false;

                        for (int k = 0; k < edges.Count; k++)
                        {
                            if ((edges[k][0] == currentPoint || edges[k][1] == currentPoint) && (edges[k][0] == tmp1.VertexA || edges[k][1] == tmp1.VertexA))
                            {
                                edgeAExist = true;
                            }
                            if ((edges[k][0] == currentPoint || edges[k][1] == currentPoint) && (edges[k][0] == tmp1.VertexB || edges[k][1] == tmp1.VertexB))
                            {
                                edgeBExist = true;
                            }
                            if ((edges[k][0] == currentPoint || edges[k][1] == currentPoint) && (edges[k][0] == tmp1.VertexC || edges[k][1] == tmp1.VertexC))
                            {
                                edgeCExist = true;
                            }
                        }
                        if (!edgeAExist)
                        {
                            edges.Add(new Vector[] { currentPoint, tmp1.VertexA });
                        }
                        if (!edgeBExist)
                        {
                            edges.Add(new Vector[] { currentPoint, tmp1.VertexB });
                        }
                        if (!edgeCExist)
                        {
                            edges.Add(new Vector[] { currentPoint, tmp1.VertexC });
                        }


                    }
                    if (!(tmp2.VertexA == null))
                    {

                        if (tmp2.VertexA != tmp1.VertexA && tmp2.VertexA != tmp1.VertexB && tmp2.VertexA != tmp1.VertexC)
                        {
                            bool edgeExist = false;
                            for (int k = 0; k < edges.Count; k++)
                            {
                                if ((edges[k][0] == currentPoint || edges[k][1] == currentPoint) && (edges[k][0] == tmp2.VertexA || edges[k][1] == tmp2.VertexA))
                                {
                                    edgeExist = true;
                                }
                            }
                            if (!edgeExist)
                            {
                                edges.Add(new Vector[] { currentPoint, tmp2.VertexA });

                            }
                        }
                        else if (tmp2.VertexB != tmp1.VertexA && tmp2.VertexB != tmp1.VertexB && tmp2.VertexB != tmp1.VertexC)
                        {
                            bool edgeExist = false;
                            for (int k = 0; k < edges.Count; k++)
                            {
                                if ((edges[k][0] == currentPoint || edges[k][1] == currentPoint) && (edges[k][0] == tmp2.VertexB || edges[k][1] == tmp2.VertexB))
                                {
                                    edgeExist = true;
                                }
                            }
                            if (!edgeExist)
                            {
                                edges.Add(new Vector[] { currentPoint, tmp2.VertexB });

                            }
                        }
                        else if (tmp2.VertexC != tmp1.VertexA && tmp2.VertexC != tmp1.VertexB && tmp2.VertexC != tmp1.VertexC)
                        {
                            bool edgeExist = false;
                            for (int k = 0; k < edges.Count; k++)
                            {
                                if ((edges[k][0] == currentPoint || edges[k][1] == currentPoint) && (edges[k][0] == tmp2.VertexC || edges[k][1] == tmp2.VertexC))
                                {
                                    edgeExist = true;
                                }
                            }
                            if (!edgeExist)
                            {
                                edges.Add(new Vector[] { currentPoint, tmp2.VertexC });

                            }
                        }
                    }
                }

            }
            DeleteEdgesWichContainsOtherEdges(edges);
        }

        private static void DeleteEdgesWichContainsOtherEdges(List<Vector[]> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                StraightLine line = StraightLine.EdgeToStraightLine(edges[i]);
                for (int j = i+1; j < edges.Count; j++)
                {
                    if (line.PointOnLine(edges[j][0]) && line.PointOnLine(edges[j][1]))
                    {
                        // lösche eine der beiden kanten
                        if (edges[i][0].X <= edges[j][0].X && edges[j][0].X <= edges[i][1].X && edges[i][0].X <= edges[j][1].X && edges[j][1].X <= edges[i][1].X 
                            || edges[i][0].X >= edges[j][0].X && edges[j][0].X >= edges[i][1].X && edges[i][0].X >= edges[j][1].X && edges[j][1].X >= edges[i][1].X)
                        {
                            if (edges[i][0].Y <= edges[j][0].Y && edges[j][0].Y <= edges[i][1].Y && edges[i][0].Y <= edges[j][1].Y && edges[j][1].Y <= edges[i][1].Y
                                || edges[i][0].Y >= edges[j][0].Y && edges[j][0].Y >= edges[i][1].Y && edges[i][0].Y >= edges[j][1].Y && edges[j][1].Y >= edges[i][1].Y)
                            {
                                edges.RemoveAt(j);
                                j--;
                            }
                        }
                        else if (edges[j][0].X <= edges[i][0].X && edges[i][0].X <= edges[j][1].X && edges[j][0].X <= edges[i][1].X && edges[i][1].X <= edges[j][1].X
                            || edges[j][0].X >= edges[i][0].X && edges[i][0].X >= edges[j][1].X && edges[j][0].X >= edges[i][1].X && edges[i][1].X >= edges[j][1].X)
                        {
                            if (edges[j][0].Y <= edges[i][0].Y && edges[i][0].Y <= edges[j][1].Y && edges[j][0].Y <= edges[i][1].Y && edges[i][1].Y <= edges[j][1].Y
                                || edges[j][0].Y >= edges[i][0].Y && edges[i][0].Y >= edges[j][1].Y && edges[j][0].Y >= edges[i][1].Y && edges[i][1].Y >= edges[j][1].Y)
                            {
                                edges.RemoveAt(j);
                                j--;
                                //break;
                            }
                        }
                    }
                }
            }
        }

       

        private static Triangle BuildSuperTriangle(List<Vector> data)
        {

            double smallestY = double.MaxValue;
            double biggestY = double.MinValue;
            double smallestX = double.MaxValue;
            double biggestX = double.MinValue;
            Vector pointOfTheSmallestX = new Vector();
            Vector pointOfTheBiggestX = new Vector();
            Vector pointOfTheBiggestY = new Vector();
            Vector pointOfTheSmallestY = new Vector();
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].X < smallestX)
                {
                    smallestX = data[i].X;
                }
                else if (data[i].X > biggestX)
                {
                    biggestX = data[i].X;
                }
                if (data[i].Y < smallestY)
                {
                    smallestY = data[i].Y;
                }
                else if (data[i].Y > biggestY)
                {
                    biggestY = data[i].Y;
                }
            }

            pointOfTheBiggestX.X = biggestX + (biggestX - smallestX);
            pointOfTheBiggestX.Y = smallestY - (biggestY - smallestY);

            pointOfTheSmallestX.X = smallestX - (biggestX - smallestX);
            pointOfTheSmallestY.Y = smallestY - (biggestY - smallestY);
            Vector smallest = new Vector(pointOfTheSmallestX.X, pointOfTheSmallestY.Y, pointOfTheSmallestY.Z);

            pointOfTheBiggestY.X = smallestX + 0.5 * (biggestX - smallestX);
            pointOfTheBiggestY.Y = biggestY + (biggestX - smallestX) + (biggestY - smallestY);

            Triangle superTriangle = new Triangle(pointOfTheBiggestX, smallest, pointOfTheBiggestY);
            return superTriangle;
        }

    }
}
