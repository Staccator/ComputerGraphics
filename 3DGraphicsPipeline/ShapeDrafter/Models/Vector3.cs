using System;

namespace ShapeDrafter.Models
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator- (Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        
        public static Vector3 operator- (Vector3 v1, Vector4 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator- (Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }
        
        public static Vector3 operator* (Vector3 v, float value)
        {
            return new Vector3(v.X * value, v.Y * value, v.Z * value);
        }
        public static Vector3 operator* (float value, Vector3 v)
        {
            return new Vector3(v.X * value, v.Y * value, v.Z * value);
        }
        
        public static Vector3 operator+ (Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public Vector3 Normalized()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            return new Vector3(X / length, Y / length, Z / length);
        }
        
        public void Normalize()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            X /= length;
            Y /= length;
            Z /= length;
        }
        
        public float DotProduct(Vector3 v)
        {
            var v1Norm = Normalized();
            var v2Norm = v.Normalized();

            return v1Norm.X*v2Norm.X+ v1Norm.Y * v2Norm.Y+ v1Norm.Z * v2Norm.Z;
        }
        
        public Vector3 CrossProduct(Vector3 v2)
            {
                return new Vector3(
                    Y * v2.Z - v2.Y * Z,
                    (X * v2.Z - v2.X * Z) * -1,
                    X * v2.Y - v2.X * Y
                    );  
            }

        public float Distance()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public void Clamp()
        {
            if (X > 1f) X = 1f;
            if (Y > 1f) Y = 1f;
            if (Z > 1f) Z = 1f;
        }
        
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}