using ShapeDrafter.MathOperations;

namespace ShapeDrafter.Models
{
    public class Vertex
    {
        public static int Width = 200;
        public static int Height = 100;

        public Vector4 Position;
        public Vector4 Homogenous;
        public Vector4 Normal;
        public Vector4 Tangent;
        public Vector4 BiNormal;
        public Vector2F TextureCoordinates;
        public ScreenPosition ScreenPos;
        public float Wc;

        public Vertex(Vector4 position, Vector4 normal)
        {
            Position = position;
            Normal = normal;
        }
        public Vertex(Vector4 position, Vector4 normal, Vector2F textureCoordinates, Vector4 newHomo)
        {
            Position = position;
            Normal = normal;
            TextureCoordinates = textureCoordinates;
            Homogenous = newHomo;
        }
        
        public Vertex(Vector4 position, Vector4 normal, ScreenPosition screenPos, float wc, Vector2F textureCoordinates, Vector4 tangent, Vector4 biNormal)
        {
            Position = position;
            Normal = normal;
            ScreenPos = screenPos;
            Wc = wc;
            TextureCoordinates = textureCoordinates;
            Tangent = tangent;
            BiNormal = biNormal;
        }

        public Vertex ApplyModelMatrix(Matrix modelMatrix, Matrix normalMatrix)
        {
            var newPosition = modelMatrix * Position;
            var result =  new Vertex(newPosition, (normalMatrix * Normal).Normalized());
            result.TextureCoordinates = TextureCoordinates;
            result.Tangent = (normalMatrix * Tangent).Normalized();
            result.BiNormal = (normalMatrix * BiNormal).Normalized();
            return result;
        }

        public Vector2F CalculateScreenCoordinates(Matrix cameraMatrix)
        {
            var homo = cameraMatrix * Position;
            homo.Z = -homo.Z;
            Homogenous = homo;
            Wc = homo.W;
            float xNDC = homo.X / Wc;
            float yNDC = homo.Y / Wc;
            float zNDC = homo.Z / Wc;

            float xs = (xNDC + 1) * Width / 2;
            float ys = (1 - yNDC) * Height / 2;
            float zs = (zNDC + 1) / 2;

            ScreenPos = new ScreenPosition((int)xs, (int)ys, zs);
            return new Vector2F(xNDC, yNDC);
        }

        public void CalculateAfterClipping()
        {
            Wc = Homogenous.W;
            float xNDC = Homogenous.X / Wc;
            float yNDC = Homogenous.Y / Wc;
            float zNDC = Homogenous.Z / Wc;

            float xs = (xNDC + 1) * Width / 2;
            float ys = (1 - yNDC) * Height / 2;
            float zs = (zNDC + 1) / 2;

            ScreenPos = new ScreenPosition((int)xs, (int)ys, zs);
        }

        public override string ToString()
        {
            return Position + "=" + Homogenous + $" = [{ScreenPos.X}, {ScreenPos.Y}, {ScreenPos.Depth}] ";
//            return Homogenous + $" = [{ScreenPos.X},{ScreenPos{Y},{ScreenPos.Depth}] ";
        }
    }
}