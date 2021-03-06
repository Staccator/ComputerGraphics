﻿using System;
using static System.Math;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Shapes;

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

        public static List<Point> GetCircle(int x0, int y0, int x1, int y1, int width, int height)
        {
            int radius = (int) new Point(x0,y0).Length(new Point(x1,y1));
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;
            int centerX = x0;
            int centerY = y0;
            List<Point> result = new List<Point>();
            
            do
            {
                if (centerX + x >= 0 && centerX + x <= width - 1 && centerY + y >= 0 && centerY + y <= height - 1) result.Add(new Point(centerX + x, centerY + y));
                if (centerX + x >= 0 && centerX + x <= width - 1 && centerY - y >= 0 && centerY - y <= height - 1) result.Add(new Point(centerX + x, centerY - y));
                if (centerX - x >= 0 && centerX - x <= width - 1 && centerY + y >= 0 && centerY + y <= height - 1) result.Add(new Point(centerX - x, centerY + y));
                if (centerX - x >= 0 && centerX - x <= width - 1 && centerY - y >= 0 && centerY - y <= height - 1) result.Add(new Point(centerX - x, centerY - y));
                if (centerX + y >= 0 && centerX + y <= width - 1 && centerY + x >= 0 && centerY + x <= height - 1) result.Add(new Point(centerX + y, centerY + x));
                if (centerX + y >= 0 && centerX + y <= width - 1 && centerY - x >= 0 && centerY - x <= height - 1) result.Add(new Point(centerX + y, centerY - x));
                if (centerX - y >= 0 && centerX - y <= width - 1 && centerY + x >= 0 && centerY + x <= height - 1) result.Add(new Point(centerX - y, centerY + x));
                if (centerX - y >= 0 && centerX - y <= width - 1 && centerY - x >= 0 && centerY - x <= height - 1) result.Add(new Point(centerX - y, centerY - x));
                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);

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