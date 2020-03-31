namespace ShapeDrafter.Models
{
    public struct ScreenPosition
    {
        public bool Equals(ScreenPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is ScreenPosition other && Equals(other);
        }

        public static bool operator ==(ScreenPosition left, ScreenPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ScreenPosition left, ScreenPosition right)
        {
            return !left.Equals(right);
        }

        public int X;
        public int Y;
        public float Depth;

        public ScreenPosition(int x, int y, float depth)
        {
            X = x;
            Y = y;
            Depth = depth;
        }
    }
}