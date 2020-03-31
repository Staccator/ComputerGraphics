using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public void DrawPoints(List<Point> points, Color color, Color[] colors = null)
        {
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var x = point.X;
                var y = point.Y;

                if (x < 0 || x >= _width || y < 0 || y >= _height)
                    continue;

                if (colors != null)
                {
                    color = colors[i];
                }

                var pixelOffset = CalculatePixelOffset(x, y);
                _sourceBuffer[pixelOffset] = color.B;
                _sourceBuffer[pixelOffset + 1] = color.G;
                _sourceBuffer[pixelOffset + 2] = color.R;
                _sourceBuffer[pixelOffset + 3] = color.A;
            }
        }
        private int CalculatePixelOffset(int x, int y)
        {
            return (x + _wb.PixelWidth * y) * (_wb.Format.BitsPerPixel / 8);
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


        public void RedrawTexture()
        {
            _mainTexture.GetAllPixels(out List<Point> points, out List<Color> colors);
            DrawPoints(points, Color.Bisque, colors.ToArray());
            CommitDraw();
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
        
                return bitmapimage;
            }
        }
    }
}