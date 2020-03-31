using System;
using System.Collections.Generic;
using System.Linq;
using ShapeDrafter.Models;

namespace ShapeDrafter.MathOperations
{
    public static class Renderer
    {
        private static float Interpolate(float z1, float z2, float q)
        {
            return z1 * (1 - q) + z2 * q;
        }
        
        public static Vector3 Interpolate(Vector3 v1, Vector3 v2, float q)
        {
            return new Vector3(Interpolate(v1.X, v2.X, q), Interpolate(v1.Y, v2.Y, q), Interpolate(v1.Z, v2.Z, q));
        }

        private static Vector4 Interpolate(Vector4 v1, Vector4 v2, float q)
        {
            return new Vector4(Interpolate(v1.X, v2.X, q), Interpolate(v1.Y, v2.Y, q), Interpolate(v1.Z, v2.Z, q), 0);
        }
        public static Vector2F Interpolate(Vector2F v1, Vector2F v2, float q)
        {
            return new Vector2F(Interpolate(v1.X, v2.X, q), Interpolate(v1.Y, v2.Y, q));
        }

        private static Vector4 Interpolate(Vector4 v1, Vector4 v2, float left, float right, float bot)
        {
            float newX = (left * v1.X + right * v2.X) / bot;
            float newY = (left * v1.Y + right * v2.Y) / bot;
            float newZ = (left * v1.Z + right * v2.Z) / bot;
            return new Vector4(newX, newY, newZ, 0);
        }

        private static float Interpolate(float v1, float v2, float left, float right, float bot)
        {
            return (left * v1 + right * v2) / bot;
        }

        private static Vertex Interpolation(Vertex v1, Vertex v2, float q, int x, int y)
        {
            if (GlobalOptions.Perspective)
            {
                float wc1 = v1.Wc;
                float wc2 = v2.Wc;
                float left = (1 - q) / wc1;
                float right = q / wc2;
                float bot = left + right;
                //Position and Normal Interpolation
                var newPosition = Interpolate(v1.Position, v2.Position, left, right, bot);
                var newNormal = Interpolate(v1.Normal, v2.Normal, left, right, bot).Normalized();
                var newTangent = Interpolate(v1.Tangent, v2.Tangent, left, right, bot).Normalized();
                var newBiNormal = Interpolate(v1.BiNormal, v2.BiNormal, left, right, bot).Normalized();
                //Depth interpolation
                float newWc = Interpolate(v1.Wc, v2.Wc, left, right, bot);
                float textureX = Interpolate(v1.TextureCoordinates.X, v2.TextureCoordinates.X, left, right, bot);
                float textureY = Interpolate(v1.TextureCoordinates.Y, v2.TextureCoordinates.Y, left, right, bot);
                float newDepth = Interpolate(v1.ScreenPos.Depth, v2.ScreenPos.Depth, q);
                var screenPos = new ScreenPosition(x, y, newDepth);
                return new Vertex(newPosition, newNormal, screenPos, newWc, new Vector2F(textureX, textureY), newTangent, newBiNormal);
            }
            else
            {
                //Position and Normal Interpolation
                var newPosition = Interpolate(v1.Position, v2.Position, q);
                var newNormal = Interpolate(v1.Normal, v2.Normal, q).Normalized();
                var newTangent = Interpolate(v1.Tangent, v2.Tangent, q).Normalized();
                var newBiNormal = Interpolate(v1.BiNormal, v2.BiNormal, q).Normalized();
                //Depth interpolation
                float newWc = Interpolate(v1.Wc, v2.Wc, q);
                float textureX = Interpolate(v1.TextureCoordinates.X, v2.TextureCoordinates.X, q);
                float textureY = Interpolate(v1.TextureCoordinates.Y, v2.TextureCoordinates.Y, q);
                float newDepth = Interpolate(v1.ScreenPos.Depth, v2.ScreenPos.Depth, q);
                var screenPos = new ScreenPosition(x, y, newDepth);
                return new Vertex(newPosition, newNormal, screenPos, newWc, new Vector2F(textureX, textureY), newTangent, newBiNormal);
            }
        }
        
        public static List<Vertex> Rasterize(Triangle triangle)
        {
            bool fillInterior = GlobalOptions.Filling;
            List<Vertex> result = new List<Vertex>();
            
            var sorted = new List<Vertex>() {triangle.V1, triangle.V2, triangle.V3}.OrderBy(v => v.ScreenPos.Y).ToArray();
            var top = sorted[2];
            var mid = sorted[1];
            var bot = sorted[0];
            int triangleHeight = top.ScreenPos.Y - bot.ScreenPos.Y;
            int midHeight = mid.ScreenPos.Y - bot.ScreenPos.Y;
            int aboveMidHeight = top.ScreenPos.Y - mid.ScreenPos.Y;
            float qMid = midHeight / (float)triangleHeight;
            float mid2X = Interpolate(bot.ScreenPos.X, top.ScreenPos.X, qMid);
            var mid2 = Interpolation(bot, top, qMid, (int)mid2X, mid.ScreenPos.Y);
            
            Vertex midLeft, midRight;
            if (mid.ScreenPos.X < mid2.ScreenPos.X)
            { midLeft = mid; midRight = mid2; }
            else
            { midLeft = mid2; midRight = mid; }

            //Bottom Triangle
            float qBotLeft = (bot.ScreenPos.X - midLeft.ScreenPos.X) / (float) midHeight;
            float qBotRight = (bot.ScreenPos.X - midRight.ScreenPos.X) / (float) midHeight;
            float xLeft = midLeft.ScreenPos.X;
            float xRight = midRight.ScreenPos.X;
            float qVertical = 0;
            float diffQVertical = 1f / midHeight;
            
            for (int y = midLeft.ScreenPos.Y; y >= bot.ScreenPos.Y ; y--)
            {
                int xLeftInt = (int) xLeft;
                int xRightInt = (int) xRight;
                var vertexLeft = Interpolation(midLeft, bot, qVertical, xLeftInt, y);
                var vertexRight = Interpolation(midRight, bot, qVertical, xRightInt, y);
                
                result.Add(vertexLeft);
                
                if (fillInterior || y == bot.ScreenPos.Y)
                {
                    float qHorizontal = 0;
                    float diffQHorizontal = 1f / (xRightInt - xLeftInt);
                    for (int x = xLeftInt + 1; x < xRightInt; x++)
                    {
                        qHorizontal += diffQHorizontal;
                        var interpolatedVertex = Interpolation(vertexLeft, vertexRight, qHorizontal, x, y);
                        result.Add(interpolatedVertex);
                    }
                }
                
                if (xLeftInt != xRightInt)
                    result.Add(vertexRight);

                qVertical += diffQVertical;
                xLeft += qBotLeft;
                xRight += qBotRight;
            }

            //Top Triangle
            float qTopLeft = (top.ScreenPos.X - midLeft.ScreenPos.X) / (float) aboveMidHeight;
            float qTopRight = (top.ScreenPos.X - midRight.ScreenPos.X) / (float) aboveMidHeight;
            xLeft = midLeft.ScreenPos.X;
            xRight = midRight.ScreenPos.X;
            qVertical = 0;
            diffQVertical = 1f / aboveMidHeight;
            
            for (int y = midLeft.ScreenPos.Y + 1; y <= top.ScreenPos.Y ; y++)
            {
                qVertical += diffQVertical;
                xLeft += qTopLeft;
                xRight += qTopRight;
                
                int xLeftInt = (int) xLeft;
                int xRightInt = (int) xRight;
                var vertexLeft = Interpolation(midLeft, top, qVertical, xLeftInt, y);
                var vertexRight = Interpolation(midRight, top, qVertical, xRightInt, y);
                
                result.Add(vertexLeft);

                if (fillInterior || y == top.ScreenPos.Y)
                {
                    float qHorizontal = 0;
                    float diffQHorizontal = 1f / (xRightInt - xLeftInt);
                    for (int x = xLeftInt + 1; x < xRightInt; x++)
                    {
                        qHorizontal += diffQHorizontal;
                        var interpolatedVertex = Interpolation(vertexLeft, vertexRight, qHorizontal, x, y);
                        result.Add(interpolatedVertex);
                    }
                }
                
                if (xLeftInt != xRightInt)
                    result.Add(vertexRight);
            }

            return result;
        }
        
    }
}