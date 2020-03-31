using System.Collections.Generic;
using ShapeDrafter.Graphics;
using ShapeDrafter.MathOperations;

namespace ShapeDrafter.Models
{
    public class Triangle
    {
        public Vertex V1;
        public Vertex V2;
        public Vertex V3;
        public int number;
        public bool BackFaced;

        public List<Vertex> List => new List<Vertex>{V1, V2, V3};

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            number = -1;
            BackFaced = false;
        }

        public Triangle ApplyModelMatrix(Matrix modelMatrix, Matrix normalMatrix)
        {
            var v1 = V1.ApplyModelMatrix(modelMatrix, normalMatrix);
            var v2 = V2.ApplyModelMatrix(modelMatrix, normalMatrix);
            var v3 = V3.ApplyModelMatrix(modelMatrix, normalMatrix);
            return new Triangle(v1, v2, v3);
        }

        public void CalculateScreenCoordinates(Matrix cameraMatrix)
        {
            var v1F = V1.CalculateScreenCoordinates(cameraMatrix);
            var v2F = V2.CalculateScreenCoordinates(cameraMatrix);
            var v3F = V3.CalculateScreenCoordinates(cameraMatrix);
            BackFaced = !Geometry.IsTriangleClockwise(v1F, v2F, v3F);
        }

        public override string ToString()
        {
            return $"TRI[{number}] {V1} ||| {V2} ||| {V3}";
        }

        public bool IsGoodTriangle()
        {
            return !(V1.ScreenPos == V2.ScreenPos || V2.ScreenPos == V3.ScreenPos || V3.ScreenPos == V1.ScreenPos
                     || V1.ScreenPos.X == V2.ScreenPos.X && V2.ScreenPos.X == V3.ScreenPos.X
                     || V1.ScreenPos.Y == V2.ScreenPos.Y && V2.ScreenPos.Y == V3.ScreenPos.Y
                     );
        }
    }
}