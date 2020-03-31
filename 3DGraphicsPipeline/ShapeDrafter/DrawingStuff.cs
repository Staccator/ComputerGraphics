using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using ShapeDrafter.Graphics;
using ShapeDrafter.Models;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private int _width;
        private int _height;
        private readonly byte[] _sourceBuffer;
        private Int32Rect _sourceRect;
        private void SetupBitmap()
        {
            _wb = new WriteableBitmap(
                _width,
                _height,
                96,
                96,
                PixelFormats.Bgra32,
                null);

            PaintSurface.Source = _wb;
        }

        private void ClearScreenOld()
        {
            var points = new List<Point>();
            for (var i = 0; i < _width; i++)
            for (var j = 0; j < _height; j++)
                points.Add(new Point(i, j));
            DrawPoints(points, Color.White);
        }

        private void ClearScreen()
        {
            Parallel.For(0, _width * _height * 4, (i) => { _sourceBuffer[i] = 5; });
            // for (var i = 0; i < _width * _height * 4; i++)
            //     _sourceBuffer[i] = 255;
        }
        private void ClearBuffer(byte[] buffer)
        {
            Parallel.For(0, _width * _height * 4, (i) => { buffer[i] = 255; });
            // for (var i = 0; i < _width * _height * 4; i++)
            //     buffer[i] = 255;
        }

        private void DrawPoints(List<Point> points, Color color, bool inside = false)
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
        }

        public void CommitDraw()
        {
            var stride = _wb.PixelWidth * (_wb.Format.BitsPerPixel / 8);
            _wb.WritePixels(_sourceRect, _sourceBuffer, stride, 0);
        }
        
        public void CommitDrawWithBuffer(byte[] buffer)
        {
            var stride = _wb.PixelWidth * (_wb.Format.BitsPerPixel / 8);
            _wb.WritePixels(_sourceRect, buffer, stride, 0);
        }

        private int CalculatePixelOffset(int x, int y)
        {
            return (x + _wb.PixelWidth * y) * (_wb.Format.BitsPerPixel / 8);
        }

        private void DrawTexels(List<Texel> texels, byte[] buffer, float[,] depthBuffer)
        {
            for (var i = 0; i < texels.Count; i++)
            {
                var texel = texels[i];
                var x = texel.Position.X;
                var y = texel.Position.Y;
                if (x < 0 || x >= _width || y < 0 || y >= _height)
                    continue;

                if (GlobalOptions.ZBuffering)
                {
                    if (texel.Depth < depthBuffer[x, y])
                        continue;
                    depthBuffer[x, y] = texel.Depth;
                }

                var color = texel.Color;

                var pixelOffset = (x + _width * y) * 4;
                buffer[pixelOffset] = color.B;
                buffer[pixelOffset + 1] = color.G;
                buffer[pixelOffset + 2] = color.R;
                buffer[pixelOffset + 3] = color.A;
            }
        }

        private byte[] CreateNewBuffer()
        {
            return new byte[_width * _height * (_wb.Format.BitsPerPixel / 8)];
        }

        private float[,] CreateNewDepthBuffer()
        {
            return new float[_width, _height];
        }
    }
}