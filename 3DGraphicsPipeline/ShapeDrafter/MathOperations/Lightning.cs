using System;
using System.Collections.Generic;
using System.Drawing;
using ShapeDrafter.Graphics;
using ShapeDrafter.Models;

namespace ShapeDrafter.MathOperations
{
    public static class Lightning
    {
        public static Vector3 CameraPosition = new Vector3(0, 0, 10);
        public static List<Light> Lights = new List<Light>();
        public static bool Texturing = false;
        public static Texture Texture;
        public static bool BumpMapping = false;
        public static Texture BumpMap;

        public const float Ka = 0.4f;
        public const float Kd = 0.55f;
        public const float Ks = 0.2f;
        public const float Alpha = 1.2f;
        public static readonly Vector3 Ia = new Vector3(0.4f, 0.4f, 0.4f);
        public static readonly Vector3 Id = new Vector3(0.8f, 0.4f, 0.4f);
        public static readonly Vector3 Is = new Vector3(0.5f, 0.5f, 0.5f);

        public static Texel CalculateTexel(Vertex v)
        {
            Vector3 ambient = Ia * Ka;
            
            Vector3 N = v.Normal;
            Vector3 V = (CameraPosition - v.Position).Normalized();

            Vector3 id;
            id = Texturing ? Texture.ColorAt(v.TextureCoordinates) : Id;
            if (BumpMapping)
            {
                var mapNormal = BumpMap.ColorAt(v.TextureCoordinates);
                N =  v.Normal.ToBumpNormal(mapNormal);
                // N =  v.Normal.ApplyBumpMap(mapNormal, v.Tangent, v.BiNormal);
            }

            Vector3 lightValue = ambient;
            foreach (var light in Lights)
            {
                Vector3 L1 = (light.Position - v.Position).Normalized();
                float distance = (light.Position - v.Position).Distance();
                float dotLN = L1.DotProduct(N);
                Vector3 R = (2 * dotLN * N - L1).Normalized();
                float dotRV = R.DotProduct(V);
                var If = light.If(distance);

                Vector3 diffuse = dotLN >= 0 ? (Kd * dotLN) * id : new Vector3();
                Vector3 specular = dotRV >= 0 ? (Ks * (float) Math.Pow(dotRV, Alpha)) * light.Color : new Vector3();
                // Vector3 specular = dotRV >= 0 ? (Ks * (float) Math.Pow(dotRV, Alpha)) * Is : new Vector3();
                lightValue += (diffuse + specular) * If;
            }

            lightValue.Clamp();
            var color = Color.FromArgb((int)(lightValue.X * 255),(int)(lightValue.Y * 255),(int)(lightValue.Z * 255));
            return new Texel(new Point(v.ScreenPos.X, v.ScreenPos.Y), color, v.ScreenPos.Depth);
        }
    }
}