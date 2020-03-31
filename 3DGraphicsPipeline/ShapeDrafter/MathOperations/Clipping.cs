using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Markup;
using ShapeDrafter.Models;
// ReSharper disable LocalizableElement
// ReSharper disable HeapView.BoxingAllocation

namespace ShapeDrafter.MathOperations
{
    public static class Clipping
    {
        private static Vector4 Interpolate(Vector4 v1, Vector4 v2, float q)
        {
            return v1 * (1 - q) + v2 * q;
        }
        private static Vector2F Interpolate(Vector2F t1, Vector2F t2, float q)
        {
            return new Vector2F(t1.X * (1 - q) + t2.X * q, t1.Y * (1 - q) + t2.Y * q);
        }

        private static Vertex Interpolation(Vertex v1, Vertex v2, float q)
        {
            var newPosition = Interpolate(v1.Position, v2.Position, q);
            var newHomo = Interpolate(v1.Homogenous, v2.Homogenous, q);
            var newNormal = Interpolate(v1.Normal, v2.Normal, q).Normalized();
            var textureCoords = Interpolate(v1.TextureCoordinates, v2.TextureCoordinates, q);
            
            return new Vertex(newPosition, newNormal, textureCoords, newHomo);
        }

        private static float GetDist(Vector4 h, int i)
        {
            switch (i)
            {
                case 0:
                    return h.W + h.X;
                case 1:
                    return h.W - h.X;
                case 2:
                    return h.W + h.Y;
                case 3:
                    return h.W - h.Y;
                case 4:
                    return h.W + h.Z;
                case 5:
                    return h.W - h.Z;
            }

            return 0f;
        }
        private static bool IsInsideClip(Vector4 h, int i)
        {
            switch (i)
            {
                case 0:
                    return h.X >= -h.W;
                case 1:
                    return h.X <= h.W;
                case 2:
                    return h.Y >= -h.W;
                case 3:
                    return h.Y <= h.W;
                case 4:
                    return h.Z >= -h.W;
                case 5:
                    return h.Z <= h.W;
            }
            return h.X >= -h.W && h.X <= h.W && h.Y >= -h.W && h.Y <= h.W && h.Z >= -h.W && h.Z <= h.W;
        }
        private static Vertex GetIntersect(int i , Vertex vectorA, Vertex vectorB)
        {
            float dA = GetDist(vectorA.Homogenous, i);
            float dB = GetDist(vectorB.Homogenous, i);
            float dC = dA / (dA - dB);
            return Interpolation(vectorA, vectorB, dC);
        }
        
        public static List<Triangle> ClipsTriangle(Triangle triangle)
        {
            var outputList = triangle.List;
            
            for (int i = 0; i < 6; i++)
            {
                List<Vertex> inputList = outputList.ToList();
                outputList.Clear();
                if (inputList.Count == 0) break;
 
                var S = inputList[inputList.Count - 1];
 
                foreach (var E in inputList)
                {
                    if (IsInsideClip(E.Homogenous, i))
                    {
                        if (!IsInsideClip(S.Homogenous, i))
                        {
                            var point = GetIntersect(i, S, E);
                            outputList.Add(point);
                        }
 
                        outputList.Add(E);
                    }
                    else if (IsInsideClip(S.Homogenous, i))
                    {
                        var point = GetIntersect(i,S, E);
                        outputList.Add(point);
                    }
 
                    S = E;
                }
            }
            
            // outputList.ForEach(i => { Console.WriteLine(i.Homogenous); });
            outputList.ForEach(v => { v.CalculateAfterClipping(); });
            var result = new List<Triangle>();
            for (int i = 1; i < outputList.Count - 1; i++)
            {
                result.Add(new Triangle(outputList[0],outputList[i],outputList[i+1]));
            }

            // Console.WriteLine("NEW TRIANGLES");
            // result.ForEach(t=> { Console.WriteLine(t); });

            return result;
        }
    }
}