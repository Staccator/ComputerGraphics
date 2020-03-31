using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static ShapeDrafter.Graphics.Geometry;
namespace ShapeDrafter.Graphics
{
    public static class SutherlandHodgman
    {
        public static Point[] GetIntersectedPolygon(Point[] subjectPoly, Point[] clipPoly)
        {
            List<Point> outputList = subjectPoly.ToList();
            if (!IsClockwise(subjectPoly))
            {
                outputList.Reverse();
            }
 
            var clipPolyEdges = new List<Edge>();
 
            for (int i = 0; i < clipPoly.Length; i++)
            {
                var p1 = clipPoly[i];
                var p2 = clipPoly[i==clipPoly.Length-1 ? 0 : i+1];
                var edge = new Edge(p1,p2);
                clipPolyEdges.Add(edge);
            }

            foreach (Edge clipEdge in clipPolyEdges)
            {
                List<Point> inputList = outputList.ToList();
                outputList.Clear();
                if (inputList.Count == 0)
                {
                    break;
                }
 
                Point S = inputList[inputList.Count - 1];
 
                foreach (Point E in inputList)
                {
                    if (IsInside(clipEdge, E))
                    {
                        if (!IsInside(clipEdge, S))
                        {
                            Point? point = GetIntersect(S, E, clipEdge.From, clipEdge.To);
                            outputList.Add(point.Value);
                        }
 
                        outputList.Add(E);
                    }
                    else if (IsInside(clipEdge, S))
                    {
                        Point? point = GetIntersect(S, E, clipEdge.From, clipEdge.To);
                        outputList.Add(point.Value);
                    }
 
                    S = E;
                }
            }
 
            return outputList.ToArray();
        }
    }
}