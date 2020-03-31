using System;
using System.Windows;

namespace ShapeDrafter.Graphics
{
    public class Edge
    {
        public readonly Point From;
        public readonly Point To;

        public Edge(Point from, Point to)
        {
            From = from;
            To = to;
        }
    }

    public static class Geometry
    {
        private const double Tolerance = .000001d;

        public static Point? GetIntersect(Point line1From, Point line1To, Point line2From, Point line2To)
        {
            var direction1 = line1To - line1From;
            var direction2 = line2To - line2From;
            var dotPerp = direction1.X * direction2.Y - direction1.Y * direction2.X;

            if (Math.Abs(dotPerp) <= Tolerance) return null;

            var c = line2From - line1From;
            var t = (c.X * direction2.Y - c.Y * direction2.X) / dotPerp;
            return line1From + t * direction1;
        }

        public static bool IsClockwise(Point[] polygon)
        {
            for (var cntr = 2; cntr < polygon.Length; cntr++)
            {
                var isLeft = IsLeftOf(new Edge(polygon[0], polygon[1]), polygon[cntr]);
                if (isLeft != null
                )
                    return !isLeft.Value;
            }

            throw new ArgumentException("colinear");
        }

        public static bool IsInside(Edge edge, Point test)
        {
            var isLeft = IsLeftOf(edge, test);
            if (isLeft == null) return true;

            return !isLeft.Value;
        }

        public static bool? IsLeftOf(Edge edge, Point point)
        {
            var first = edge.To - edge.From;
            var second = point - edge.To;
            var dotproduct = first.X * second.Y - first.Y * second.X;
            if (Math.Abs(dotproduct) < Tolerance) return null;
            return dotproduct > 0;
        }
    }
}