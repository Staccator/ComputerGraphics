using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace ShapeDrafter.Models
{
    public class Texture
    {
        public readonly Color[,] ColorTab;
        public readonly Color[,] TempColorTab;
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<Point> _points = new List<Point>();
        public int Width { get; set; }
        public int Height { get; set; }

        public Texture()
        {
            ColorTab = new Color[800, 600];
            for (var i = 0; i < 800; i++)
            for (var j = 0; j < 600; j++)
                ColorTab[i, j] = Color.Black;
        }

        public Texture(Bitmap bmp)
        {
            ColorTab = new Color[bmp.Width, bmp.Height];
            TempColorTab = new Color[bmp.Width, bmp.Height];
            
            for (var i = 0; i < bmp.Width; i++)
            for (var j = 0; j < bmp.Height; j++)
            {
                var pixel = bmp.GetPixel(i, j);
                _colors.Add(pixel);
                _points.Add(new Point(i, j));
                
                ColorTab[i, j] = pixel;
                TempColorTab[i, j] = pixel;
            }

            Width = bmp.Width;
            Height = bmp.Height;
        }


        public Color[] TransformPoints(List<Point> points, double[,] matrix,int matrixMethod, double divider = 0)
        {
            var result = new Color[points.Count];
            //var sw = new Stopwatch();
            //sw.Start();
            double sum = 0;
            for (var i = 0; i < matrix.GetLength(0); i++)
            for (var j = 0; j < matrix.GetLength(1); j++)
                sum += matrix[i, j];
            if (Math.Abs(sum) < 0.001)
                sum = 0.001;
            
            if (divider != 0)
            {
                sum = divider;
            }
            
            var hmatrix = new float[,]
            {
                {-1, -2, -1},
                {0, 0, 0},
                {1, 2, 1}
            };
            var vmatrix = new float[,]
            {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };
            if (matrixMethod == 3)
            {
                hmatrix = new float[,]
                {
                    {-1, -1, 0},
                    {-1, 1, 1},
                    {0, 1, 1}
                };
                vmatrix = new float[,]
                {
                    {0, 0, 0},
                    {0, 0, 0},
                    {0, 0, 0}
                };
            }
//            ResetTempTab();
            //Console.WriteLine(sw.Elapsed);
            int n = result.Length;

            int width = ColorTab.GetLength(0);
            int height = ColorTab.GetLength(1);
            Parallel.For((int)0, (int)points.Count, (int i) =>
            {
                var point = points[i];
                Color color;
                int x = point.X;
                int y = point.Y;
                if (x < 0 || x >= width || y < 0 || y >= height)
                {}
                else
                {
                    if (matrixMethod == 3 || matrixMethod == 4)
                    {
                        color = GetEdgeTransformedPixel(point, hmatrix, vmatrix);
                    }
                    else if (matrixMethod == 5)
                    {
                        color = GetOwnMatrixTransformedPixel(point, matrix, sum);
                    }
                    else
                    {
                        color = GetTransformedPixel(point, matrix, sum);
                    }

                    TempColorTab[point.X, point.Y] = color;
                    if (i < n)
                        result[i] = color;
                }
            });
                
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
            return result;
        }

        private Color GetOwnMatrixTransformedPixel(Point point, double[,] matrix, double sum)
        {
            var x = point.X;
            var y = point.Y;
            double R = 0, G = 0, B = 0;
            var width = matrix.GetLength(0);
            var height = matrix.GetLength(1);

            int startx = -(width / 2);
            int starty = -(height / 2);
            
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var value = matrix[i, j];
                    var diffx = startx + i;
                    var diffy = starty + j;
                    var newx = (x + diffx + Width) % Width;
                    var newy = (y + diffy + Height) % Height;
                    var color = ColorTab[newx, newy];
                    R += color.R * value;
                    G += color.G * value;
                    B += color.B * value;
                }
            }

            R /= sum; G /= sum; B /= sum;
            R = Math.Min(255, Math.Max(0, R)); G = Math.Min(255, Math.Max(0, G)); B = Math.Min(255, Math.Max(0, B));
            return Color.FromArgb((int) R, (int) G, (int) B);
        }

        private Color GetTransformedPixel(Point point, double[,] matrix, double sum)
        {
            var x = point.X;
            var y = point.Y;
            double R = 0, G = 0, B = 0;
            for (var i = 0; i < matrix.GetLength(0); i++)
            for (var j = 0; j < matrix.GetLength(1); j++)
            {
                var value = matrix[i, j];
                var diffx = -1 + i;
                var diffy = -1 + j;
                var newx = (x + diffx + Width) % Width;
                var newy = (y + diffy + Height) % Height;
                var color = ColorTab[newx, newy];
                R += color.R * value;
                G += color.G * value;
                B += color.B * value;
            }

            R /= sum; G /= sum; B /= sum;
            R = Math.Min(255, Math.Max(0, R)); G = Math.Min(255, Math.Max(0, G)); B = Math.Min(255, Math.Max(0, B));
            return Color.FromArgb((int) R, (int) G, (int) B);
        }
        
        private Color GetEdgeTransformedPixel(Point point, float[,] hmatrix, float[,] vmatrix)
        {
            var x = point.X;
            var y = point.Y;
            float h=0, v=0;

            for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
            {
                var newx = (x + i + Width) % Width;
                var newy = (y + j + Height) % Height;
                var color = ColorTab[newx, newy];
                var colorValue = color.R * 0.2125f + color.B + 0.7154f + color.R * 0.0721f;
                h += hmatrix[i + 1, j + 1] * colorValue;
                v += vmatrix[i + 1, j + 1] * colorValue;
            }

            double mag = Math.Sqrt(h * h + v * v);
            mag = Math.Min(255, Math.Max(0, mag));

            return Color.FromArgb((int)mag,(int)mag,(int)mag);
        }

        public void GetAllPixels(out List<Point> points, out List<Color> colors)
        {
            points = _points;
            colors = _colors;
        }

        public void ResetTempTab()
        {
            for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
            {
                TempColorTab[i, j] = ColorTab[i, j];
            }
        }
        public void GetHistograms(out Bitmap redBitmap, out Bitmap greenBitmap, out Bitmap blueBitmap)
        {
            int histWidth = 128;
            int histHeight = 100;
            redBitmap = new Bitmap(128, 100);
            greenBitmap = new Bitmap(128, 100);
            blueBitmap = new Bitmap(128, 100);
            var rTab = new int[histWidth];
            var gTab = new int[histWidth];
            var bTab = new int[histWidth];
            
            for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
            {
                var color = TempColorTab[i, j];
                rTab[color.R / 2] += 1;
                gTab[color.G / 2] += 1;
                bTab[color.B / 2] += 1;
            }
            var rMax = (float)rTab.Max();
            var gMax = (float)gTab.Max();
            var bMax = (float)bTab.Max();

            for (int i = 0; i < histWidth; i++)
            {
                int numOfPixels = (int)(100 * rTab[i] / rMax);
                for (int j = histHeight - 1; j >= histHeight - numOfPixels; j--)
                {
                    redBitmap.SetPixel(i,j,Color.Red);
                }
                numOfPixels = (int)(100 * gTab[i] / gMax);
                for (int j = histHeight - 1; j >= histHeight - numOfPixels; j--)
                {
                    greenBitmap.SetPixel(i,j,Color.Green);
                }
                numOfPixels = (int)(100 * bTab[i] / bMax);
                for (int j = histHeight - 1; j >= histHeight - numOfPixels; j--)
                {
                    blueBitmap.SetPixel(i,j,Color.Blue);
                }
            }
        }

        public Color[] GetAreaPixels(List<Point> points)
        {
            int width = ColorTab.GetLength(0);
            int height = ColorTab.GetLength(1);
            var result = new Color[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                int x = point.X;
                int y = point.Y;
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    continue;
                }
                result[i] = ColorTab[x, y];
            }

            return result;
        }
    }
}