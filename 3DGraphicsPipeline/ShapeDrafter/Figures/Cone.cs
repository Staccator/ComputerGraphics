using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ShapeDrafter.MathOperations;
using ShapeDrafter.Models;
using Xceed.Wpf.Toolkit.Mag.Converters;

namespace ShapeDrafter.Figures
{
    public class Cone : Figure
    {
        private float _height;
        private float _radius;
        private int _phiDivision;

        public Cone(int height, float radius, int phiDivision)
        {
            _height = height;
            _radius = radius;
            _phiDivision = phiDivision;
            
            UpdateModelMatrix();
            UpdateModelTriangles();
        }

        public Cone()
        {
        }

        public float Height
        {
            get => _height;
            set
            {
                _height = value;
                UpdateModelTriangles();
            }
        }

        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                UpdateModelTriangles();
            }
        }

        public int Phi
        {
            get => _phiDivision;
            set
            {
                _phiDivision = value;
                UpdateModelTriangles();
            }
        }

        public sealed override void UpdateModelTriangles()
        {
            var newModelTriangles = new List<Triangle>();
            
            float phiStep = 2 * (float) Math.PI / _phiDivision;

            Vector4 toUp = new Vector4(0,1,0,0);
            Vector4 toDown = new Vector4(0,-1,0,0);
            
            // Vertex topVertex = new Vertex(new Vector4(0,_height,0,1), toUp);
            Vertex botVertex = new Vertex(new Vector4(0,0,0,1), toDown);
            botVertex.TextureCoordinates = new Vector2F(0.5f, 0.5f);

            float hMid = _height * _radius / (float) Math.Sqrt(_height * _height + _radius * _radius);
            float alpha = (float) Math.Atan2(_height, _radius);
            float normalRadius = hMid * (float) Math.Sin(alpha);
            float normalHeight = hMid * (float) Math.Cos(alpha);
            
            float phi = 0f;
            for (int i = 0; i < _phiDivision; i++)
            {
                float nextPhi = phi + phiStep;
                
                // Texture coords
                float texturePhi = i / (float)_phiDivision;
                float textureNextPhi = (i + 1) / (float)_phiDivision;
                Vector2F tx0 = new Vector2F(0,texturePhi);
                Vector2F tx1 = new Vector2F(1,texturePhi);
                Vector2F tx2 = new Vector2F(0,textureNextPhi);
                Vector2F txt0 = new Vector2F(((float)Math.Cos(phi) + 1)/2, ((float)Math.Sin(phi) + 1)/2);
                Vector2F txt1 = new Vector2F(((float)Math.Cos(nextPhi) + 1)/2, ((float)Math.Sin(nextPhi) + 1)/2);
                // Vector4 firstNormal = new Vector4((float)Math.Cos(phi), 0, (float)Math.Sin(phi), 0);
                // Vector4 secondNormal = new Vector4((float)Math.Cos(nextPhi), 0, (float)Math.Sin(nextPhi), 0);
                Vector4 firstNormal = GetCylinderNormal(phi, normalRadius, normalHeight);
                Vector4 midNormal = GetCylinderNormal((phi + nextPhi) / 2, normalRadius, normalHeight);
                Vector4 secondNormal = GetCylinderNormal(nextPhi, normalRadius, normalHeight);

                Vector4 left = GetCylinderPoint(nextPhi);
                Vector4 right = GetCylinderPoint(phi);
                
                //Top triangle
                Vertex leftVertex = new Vertex(left, secondNormal);
                leftVertex.TextureCoordinates = tx2;
                Vertex rightVertex = new Vertex(right, firstNormal);
                rightVertex.TextureCoordinates = tx0;
                Vertex topVertex = new Vertex(new Vector4(0,_height,0,1), toUp);//TODO
                topVertex.TextureCoordinates = tx1;
                var triangleTop = new Triangle(leftVertex, topVertex, rightVertex);
                newModelTriangles.Add(triangleTop);
                //Bot triangle
                Vertex leftVertexDown = new Vertex(left, toDown);
                leftVertexDown.TextureCoordinates = txt1;
                Vertex rightVertexDown = new Vertex(right, toDown);
                rightVertexDown.TextureCoordinates = txt0;
                var triangleBot = new Triangle(botVertex, leftVertexDown, rightVertexDown);
                newModelTriangles.Add(triangleBot);

                phi += phiStep;
            }

            ModelTriangles = newModelTriangles;
            UpdateWorldTriangles();
        }

        private Vector4 GetCylinderPoint(float phi)
        {
            return new Vector4(
                _radius * (float)Math.Cos(phi),
                0,
                _radius * (float)Math.Sin(phi),
                1);
        }
        
        private Vector4 GetCylinderNormal(float phi, float radius, float y)
        {
            return new Vector4(
                radius * (float)Math.Cos(phi),
                y,
                radius * (float)Math.Sin(phi),
                0).Normalized();
        }
    }
}