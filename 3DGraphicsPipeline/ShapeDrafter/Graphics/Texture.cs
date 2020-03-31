using System.Drawing;
using ShapeDrafter.Models;

namespace ShapeDrafter.Graphics
{
    public class Texture
    {
        public Vector3[,] texture;
        public readonly int _width;
        public readonly int _height;

        public Texture(Bitmap bmp)
        {
            _width = bmp.Width;
            _height = bmp.Height;
            texture = new Vector3[_width,_height];
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    var color = bmp.GetPixel(i, j);
                    texture[i, j] = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
                }
            }

            _widthMinusOne = _width - 1;
            _heightMinusOne = _height - 1;
        }

        public void ConvertToBumpMap()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    texture[i, j] = new Vector3(
                        texture[i,j].X * 2 - 1,
                        texture[i,j].Y * 2 - 1,
                        texture[i,j].Z * 2 - 1
                        ).Normalized();
                }
            }
        }

        private int _widthMinusOne;
        private int _heightMinusOne;
        public Vector3 ColorAt(Vector2F v)
        {
            return texture[(int)(_widthMinusOne * v.X), (int) (_heightMinusOne * v.Y)];
        }
    }
}