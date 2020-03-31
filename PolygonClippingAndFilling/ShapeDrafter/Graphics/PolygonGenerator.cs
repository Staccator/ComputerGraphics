using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ShapeDrafter.Models;

namespace ShapeDrafter.Graphics
{
    public static class PolygonGenerator
    {
        public static Random Random = new Random();

        public static ColorPolygon GeneratePolygon(int x, int y, int width, int height)
        {
            var randomPoints = new List<Point>();
            for (var i = 0; i < 10; i++) randomPoints.Add(new Point(x + Random.Next(width), y + Random.Next(height)));

            var hull = ConvexHull(randomPoints.ToArray());
            hull.Reverse();

            var filling = ScanLine.PolygonFilling(hull.Select(p => new Point(p.X - x, p.Y - y)).ToList(),out var colorTab,true,width,height);
            var polygon = new ColorPolygon(hull, Color.Black, colorTab,filling);
            polygon.Shift.Y = y;
            return polygon;
        }

        public static int Orientation(Point p, Point q, Point r)
        {
            var val = (q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y);
            if (val == 0) return 0;
            return val > 0 ? 1 : 2;
        }

        public static List<Point> ConvexHull(Point[] points)
        {
            var n = points.Length;
            if (n < 4) return null;

            var hull = new List<Point>();

            var l = 0;
            for (var i = 1; i < n; i++)
                if (points[i].X < points[l].X)
                    l = i;

            int p = l, q;

            do
            {
                hull.Add(points[p]);
                q = (p + 1) % n;
                for (var i = 0; i < n; i++)
                    if (Orientation(points[p], points[i], points[q]) == 2)
                        q = i;
                p = q;
            } while (p != l);

            return hull;
        }
    }
}