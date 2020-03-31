using System.Drawing;

namespace ShapeDrafter.Models
{
    public class Light
    {
        public Vector3 Color;
        public float Ac;
        public float Al;
        public float Aq;
        public Vector3 Position;
        
        public Light(Vector3 color, float ac, float al, float aq, Vector3 position)
        {
            Color = color;
            Ac = ac;
            Al = al;
            Aq = aq;
            Position = position;
        }

        public Light()
        {
        }

        public float If(float dist)
        {
            return 1 / (Ac + Al * dist + Aq * dist * dist);
        }
    }
}