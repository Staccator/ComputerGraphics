using System;

namespace ShapeDrafter.Models
{
    public struct Vector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4 operator- (Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }

        public static Vector4 operator- (Vector4 v)
        {
            return new Vector4(-v.X, -v.Y, -v.Z, -v.W);
        }
        
        public static Vector4 operator* (Vector4 v, float value)
        {
            return new Vector4(v.X * value, v.Y * value, v.Z * value, v.W * value);
        }
        
        public static Vector4 operator+ (Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }
        
        public static implicit operator Vector3 (Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public void Normalize()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            X /= length;
            Y /= length;
            Z /= length;
        }
        
        public Vector4 Normalized()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            return new Vector4(X / length, Y / length, Z / length, 0);
        }
        
        public Vector4 CrossProduct(Vector4 v2)
        {
            return new Vector4(
                Y * v2.Z - v2.Y * Z,
                (X * v2.Z - v2.X * Z) * -1,
                X * v2.Y - v2.X * Y,
                0
                );  
        }
        public Vector3 CrossProduct(Vector3 v2)
        {
            return new Vector3(
                Y * v2.Z - v2.Y * Z,
                (X * v2.Z - v2.X * Z) * -1,
                X * v2.Y - v2.X * Y
            );  
        }
        
        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }

        public Vertex ToSphereVertex()
        {
            return new Vertex(this, Normalized());
        }

        private static Vector3 _randomVec = new Vector3(0.01f,1,0).Normalized();
        public Vector3 ToBumpNormal(Vector3 Nm)
        {
            var B = _randomVec.CrossProduct(this).Normalized();
            var T = this.CrossProduct(B);
            float x = T.X * Nm.X + B.X * Nm.Y + X * Nm.Z;
            float y = T.Y * Nm.Y + B.Y * Nm.Y + Y * Nm.Z;
            float z = T.Z * Nm.X + B.Z * Nm.Y + Z * Nm.Z;
            return new Vector3(x,y,z).Normalized();
        }

        public Vector3 ApplyBumpMap(Vector3 Nm, Vector4 T, Vector4 B)
        {
            float x = T.X * Nm.X + B.X * Nm.Y + X * Nm.Z;
            float y = T.Y * Nm.Y + B.Y * Nm.Y + Y * Nm.Z;
            float z = T.Z * Nm.X + B.Z * Nm.Y + Z * Nm.Z;
            return new Vector3(x,y,z).Normalized();
        }
    }
}