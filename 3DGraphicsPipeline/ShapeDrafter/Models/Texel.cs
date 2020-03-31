using System.Drawing;

namespace ShapeDrafter.Models
{
    public class Texel
    {
        public Point Position;
        public Color Color;
        public float Depth;

        public Texel(Point position, Color color, float depth)
        {
            Position = position;
            Color = color;
            Depth = depth;
        }
    }
}