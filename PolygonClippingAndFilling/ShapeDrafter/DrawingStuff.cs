using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ShapeDrafter.Graphics;
using ShapeDrafter.Models;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        public readonly int _height;
        private readonly byte[] _sourceBuffer;
        private readonly Int32Rect _sourceRect;
        public readonly int _width;

        private void ClearScreen()
        {
            _started = false;
            _newPolygon = null;
            _polygons = new List<Polygon>();
            var points = new List<Point>();

            for (var i = 0; i < _width; i++)
            for (var j = 0; j < _height; j++)
                points.Add(new Point(i, j));

            DrawPoints(points, Color.White);
        }

        public void DrawPoints(List<Point> points, Color color, bool inside = false, ColorPolygon colorPolygon = null)
        {
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var x = point.X;
                var y = point.Y;

                if (x < 0 || x >= _width || y < 0 || y >= _height)
                    continue;

                if (inside)
                {
                    color = MainTexture.texture[x, y];
                    double dot = dotProducts[x, y];
                    
                    var R = ((color.R / 255d) + (_lightColor.R /255d))/2 * dot;
                    var G = ((color.G / 255d) + (_lightColor.G /255d))/2 * dot;
                    var B = ((color.B / 255d) + (_lightColor.B /255d))/2 * dot;
                    R = Math.Min(Math.Max(0, R),1);
                    G = Math.Min(Math.Max(0, G),1);
                    B = Math.Min(Math.Max(0, B),1);
                    color = Color.FromArgb((int) (255 * R), (int) (255 * G), (int) (255 * B));
                }

                if (colorPolygon != null)
                {
                    if (color != Color.White)
                    {
                        color = colorPolygon.ColorTab[x, y];
                    }
                    
                    x += colorPolygon.Shift.X;
                    y += colorPolygon.Shift.Y;
                    
                    if (x >= _width || y >= _height)
                        continue;
                }

                var pixelOffset = CalculatePixelOffset(x, y);
                _sourceBuffer[pixelOffset] = color.B;
                _sourceBuffer[pixelOffset + 1] = color.G;
                _sourceBuffer[pixelOffset + 2] = color.R;
                _sourceBuffer[pixelOffset + 3] = color.A;
            }
        }

        public static double ScalarProduct(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y + b.Y + a.Z + b.Z;
        }
        
        public static double DotProduct(Vector3D L, Vector3D N)
        {
            double LLength = Math.Sqrt(L.X * L.X + L.Y * L.Y + L.Z * L.Z);
            var LNorm = new Vector3D(L.X / LLength, L.Y / LLength, L.Z / LLength);
            double NLength = Math.Sqrt(N.X * N.X + N.Y * N.Y + N.Z * N.Z);
            var NNorm = new Vector3D(N.X / NLength, N.Y / NLength, N.Z / NLength);

            return LNorm.X*NNorm.X+ LNorm.Y * NNorm.Y+ LNorm.Z * NNorm.Z;
        }

        public void CommitDraw()
        {
            var stride = _wb.PixelWidth * (_wb.Format.BitsPerPixel / 8);
            _wb.WritePixels(_sourceRect, _sourceBuffer, stride, 0);
        }

        public void DrawDot(Point point, Color color)
        {
            var dotSize = 4;

            var points = new List<Point>();

            for (var i = point.X - dotSize; i < point.X + dotSize; i++)
            for (var j = point.Y - dotSize; j < point.Y + dotSize; j++)
                points.Add(new Point(i, j));

            DrawPoints(points, color);
        }

        private int CalculatePixelOffset(int x, int y)
        {
            return (x + _wb.PixelWidth * y) * (_wb.Format.BitsPerPixel / 8);
        }

        private List<Point> lastfilling = new List<Point>();
        public void RedrawAll()
        {
            
            foreach (var polygon in _polygons)
            {
                polygon.Redraw();
//                DrawPoints(lastfilling,Color.White);
//                lastfilling = ScanLine.PolygonFilling(polygon.Vertices.Select(v => v.Point).ToList());
//                DrawPoints(lastfilling,Color.Red);
            }
            
            foreach (var polygon in _generated)
            {
//                polygon.Redraw();
                DrawPoints(polygon.Interior,Color.Green,false,polygon);
            }

            foreach (var polygon in _clipped)
            {
                DrawPoints(polygon.Points, Color.RoyalBlue,true);
            }

            CommitDraw();
        }

        private void CreateBumpMap(Bitmap bmp)
        {
            BumpMap = new Vector3D[Texture.Width, Texture.Height];

            for (var i = 0; i < Texture.Width; i++)
            for (var j = 0; j < Texture.Height; j++)
            {
                var pixel = bmp.GetPixel(i % bmp.Width, j % bmp.Height);
                var pixelright = bmp.GetPixel((i + 1) % bmp.Width, j % bmp.Height);
                var pixelbot = bmp.GetPixel(i % bmp.Width, (j + 1) % bmp.Height);
                var grayness = (double) pixel.R / 255;
                var graynessright = (double) pixelright.R / 255;
                var graynessbot = (double) pixelbot.R / 255;
                var dx = graynessright - grayness;
                var dy = graynessbot - grayness;
                var vec3 = new Vector3D();
                double dLength = Math.Sqrt(dx * dx + dy * dy);
                if (dLength != 0)
                {
                    vec3.X = -(dx / dLength);
                    vec3.Y = -(dy / dLength);
                }

                BumpMap[i, j] = vec3;
            }
        }
    }
}