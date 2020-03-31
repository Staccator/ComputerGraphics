namespace ShapeDrafter.Models
{
    public struct Intersection
    {
        public Vector4 Vector;
        public int Hyper;
        public float Q;

        public Intersection(Vector4 vector, int hyper, float q)
        {
            Vector = vector;
            Hyper = hyper;
            Q = q;
        }

        public override string ToString()
        {
            return $"H {Hyper}, {Q} Inter{Vector}";
        }
    }
}