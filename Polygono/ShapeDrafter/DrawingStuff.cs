using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using ShapeDrafter.Models;
using Point = System.Drawing.Point;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        public readonly int _height;
        public readonly int _width;
        private readonly byte[] _sourceBuffer;
        private readonly Int32Rect _sourceRect;

        private void ClearScreen()
        {
            _started = false;
            _circles = new List<Circle>();
            _first = false;
            _newPolygon = null;
            _polygons = new List<Polygon>();
            var points = new List<Point>();

            for (var i = 0; i < _width; i++)
            for (var j = 0; j < _height; j++)
                points.Add(new Point(i, j));

            DrawPoints(points, Color.White);
        }

        public void DrawPoints(List<Point> points, Color color)
        {
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var x = point.X;
                var y = point.Y;

                if (x < 0 || x >= _width || y < 0 || y >= _height)
                    continue;

                var pixelOffset = CalculatePixelOffset(x, y);
                _sourceBuffer[pixelOffset] = color.B;
                _sourceBuffer[pixelOffset + 1] = color.G;
                _sourceBuffer[pixelOffset + 2] = color.R;
                _sourceBuffer[pixelOffset + 3] = color.A;
            }

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

        public void RedrawAll()
        {
            foreach (var polygon in _polygons)
            {
               polygon.Redraw(); 
            }

            foreach (var circle in _circles)
            {
                circle.Redraw();
            }
        }
    }
}