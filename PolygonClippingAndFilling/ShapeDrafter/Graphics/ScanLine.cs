using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Documents;

namespace ShapeDrafter.Graphics
{
    public static class ScanLine
    {
        public static Color Mix(this Color first, Color second, float t)
        {
            float R = first.R + (second.R - first.R) * t;
            float G = first.G + (second.G - first.G) * t;
            float B = first.B + (second.B - first.B) * t;
            if (R < 0 || R > 255 || B < 0 || B > 255 || G < 0 || G > 255)
            {
                return Color.White;
            }
            return Color.FromArgb((int) R, (int) G, (int) B);
        }
        private class Edge
        {
            public readonly Point From;
            public readonly Point To;
            public double xmin;
            public double dxdy;
            public Edge(Point from, Point to)
            {
                From = from;
                To = to;
                if (from.X < to.X)
                {
                    double dy = (to.Y - from.Y);
                    dxdy = dy == 0 ? 0 : (double)(to.X - from.X) / (dy);
                }
                else
                {
                    double dy = (from.Y - to.Y);
                    dxdy = dy == 0 ? 0 : (double)(from.X - to.X) / (dy);
                }

                if (from.Y < to.Y)
                {
                    xmin = from.X;
                }
                else
                {
                    xmin = to.X;
                }
            }
        }

        public static Random Random = new Random();
        public static List<Point> PolygonFilling(List<Point> points, out Color[,] colorTab, bool colors = false,int width = 200, int height = 200)
        {
            int n = points.Count;
            colorTab = null;
            for (int i = 0; i < n; i++)
            {
                var point1 = points[i];
                var point2 = points[(i+1) % n];
                if (point1.Y == point2.Y)
                {
                    points[(i+1)%n] = new Point(point2.X,point2.Y - 1);
                } 
            }
            
            var ind = points.Select((p, index) => new {p, index}).OrderBy(p => p.p.Y).Select(p => p.index).ToArray();
            
            Dictionary<Point,Color> dict = new Dictionary<Point, Color>();
            if (colors)
            {
                var randColors = new List<Color>(){Color.Blue,Color.Green,Color.Red,Color.Yellow,Color.Violet,Color.Crimson,Color.DarkBlue};
                colorTab = new Color[width,height];
                for (int i = 0; i < n; i++)
                {
                    var randColor = randColors[Random.Next(randColors.Count)];
                    dict.Add(points[i],randColor);
                }
            }
            
            List<Edge> AET = new List<Edge>();
            List<Point> result = new List<Point>();

            int ymin = points[ind[0]].Y;
            int ymax = points[ind[n-1]].Y;
            int k = 0;
            for (int y = ymin; y <= ymax; y++)
            {
                while (points[ind[k]].Y == y-1)
                {
                    int i = ind[k];
                    k++;
                    Point P = points[i];
                    Point Pprev = i == 0 ? points[n-1] : points[i-1];
                    if (Pprev.Y >= P.Y)
                    {
                        AET.Add(new Edge(Pprev,P));
                    }
                    else
                    {
                        var toRemove = AET.First(e => e.From == P && e.To == Pprev || e.From == Pprev && e.To == P);
                        AET.Remove(toRemove);
                    }
                    
                    Point Pnext = i == n-1 ? points[0] : points[i+1];
                    if (Pnext.Y >= P.Y)
                    {
                        AET.Add(new Edge(Pnext,P));
                    }
                    else
                    {
                        var toRemove = AET.First(e => e.From == P && e.To == Pnext || e.From == Pnext && e.To == P);
                        AET.Remove(toRemove);
                    }
                }

                AET = AET.OrderBy(e => e.xmin).ToList();
                int m = AET.Count;
                for (int i = 0; i < m; i+=2)
                {
                    var edge1 = AET[i];
                    var edge2 = AET[i + 1];
                    var x1 = (int)edge1.xmin;
                    var x2 = (int)edge2.xmin;

                    if (colors)
                    {
                        Color mid1, mid2;
                        if (edge1.From.Y < edge1.To.Y)
                        {
                            var colortop = dict[edge1.To];
                            var colorbot = dict[edge1.From];
                            float diffy = edge1.To.Y - edge1.From.Y;
                            mid1 = colorbot.Mix(colortop, (y - edge1.From.Y) / diffy);
                        }
                        else
                        {
                            var colortop = dict[edge1.From];
                            var colorbot = dict[edge1.To];
                            float diffy = edge1.From.Y - edge1.To.Y;
                            mid1 = colorbot.Mix(colortop, (y - edge1.To.Y) / diffy);
                        }
                        if (edge2.From.Y < edge2.To.Y)
                        {
                            var colortop = dict[edge2.To];
                            var colorbot = dict[edge2.From];
                            float diffy = edge2.To.Y - edge2.From.Y;
                            mid2 = colorbot.Mix(colortop, (y - edge2.From.Y) / diffy);
                        }
                        else
                        {
                            var colortop = dict[edge2.From];
                            var colorbot = dict[edge2.To];
                            float diffy = edge2.From.Y - edge2.To.Y;
                            mid2 = colorbot.Mix(colortop, (y - edge2.To.Y) / diffy);
                        }
                        
                        for (int j = x1; j < x2; j++)
                        {
                            result.Add(new Point(j,y));
                            var t = (float)(j-x1)/(x2-x1);
                            colorTab[j, y] = mid1.Mix(mid2, t);
                        }

                        colorTab[x2, y] = mid2;

                    }
                    else
                    {
                        for (int j = x1; j <= x2; j++)
                        {
                            result.Add(new Point(j,y));
                        }
                    }
                }
                for (int i = 0; i < m; i++)
                {
                    var edge = AET[i];
                    edge.xmin += edge.dxdy;
                }
            }

            return result;
        }
    }
}