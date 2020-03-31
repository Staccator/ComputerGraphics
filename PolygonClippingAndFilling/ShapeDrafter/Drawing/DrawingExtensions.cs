using System;
using static System.Math;
using System.Collections.Generic;
using System.Drawing;

namespace ShapeDrafter.Drawing
{
    public static class DrawingExtensions
    {
        public static List<Point> GetLine(int x0, int y0, int x1, int y1)
        {
            var result = new List<Point>();

            int dx = Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            var err = (dx > dy ? dx : -dy) / 2;
            
            while (true)
            {
                result.Add(new Point(x0, y0));
                if (x0 == x1 && y0 == y1) break;
                var e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dy)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return result;
        }

        public static double Length(this Point point1, Point point2)
        {
            return Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }
        
        public static Point Add(this Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }
        
        public static Point Minus(this Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }
        
        public static Point Multiply(this Point point1, double scalar)
        {
            return new Point((int) (point1.X * scalar), (int) (point1.Y * scalar));
        }
    }
}