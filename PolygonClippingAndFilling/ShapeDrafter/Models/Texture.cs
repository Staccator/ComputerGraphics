using System.Drawing;

namespace ShapeDrafter.Models
{
    public class Texture
    {
        public Color[,] texture;
        public const int Width = 800;
        public const int Height = 600;

        public Texture()
        {
            texture = new Color[Width,Height];
            for (int i = 0; i < Texture.Width; i++)
            {
                for (int j = 0; j < Texture.Height; j++)
                {
                    texture[i, j] = Color.Black;
                }
            }
        }
        public Texture(Bitmap bmp)
        {
            texture = new Color[Width,Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    texture[i, j] = bmp.GetPixel(i%bmp.Width, j%bmp.Height);
                }
            }
        }
    }
}