using System;
using System.Collections.Generic;
using ShapeDrafter.Models;

namespace ShapeDrafter.Figures
{
    public class Cylinder : Figure
    {
        private float _height;
        private float _radius;
        private int _phiDivision;

        public Cylinder(int height, float radius, int phiDivision)
        {
            _height = height;
            _radius = radius;
            _phiDivision = phiDivision;
            
            UpdateModelMatrix();
            UpdateModelTriangles();
        }

        public Cylinder()
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
            
            Vertex top = new Vertex(new Vector4(0,_height / 2,0,1), toUp);
            top.TextureCoordinates = new Vector2F(0.5f, 0.5f);
            Vertex bot = new Vertex(new Vector4(0,-_height / 2,0,1), toDown);
            bot.TextureCoordinates = new Vector2F(0.5f, 0.5f);
            
            float phi = 0f;
            for (int i = 0; i < _phiDivision; i++)
            {
                float nextPhi = phi + phiStep;
                float texturePhi = i / (float)_phiDivision;
                float textureNextPhi = (i + 1) / (float)_phiDivision;
                Vector2F tx0 = new Vector2F(0,texturePhi);
                Vector2F tx1 = new Vector2F(1,texturePhi);
                Vector2F tx2 = new Vector2F(0,textureNextPhi);
                Vector2F tx3 = new Vector2F(1,textureNextPhi);
                
                Vector2F txt0 = new Vector2F(((float)Math.Cos(phi) + 1)/2, ((float)Math.Sin(phi) + 1)/2);
                Vector2F txt1 = new Vector2F(((float)Math.Cos(nextPhi) + 1)/2, ((float)Math.Sin(nextPhi) + 1)/2);
                
                Vector4 firstNormal = new Vector4((float)Math.Cos(phi), 0, (float)Math.Sin(phi), 0);
                Vector4 secondNormal = new Vector4((float)Math.Cos(nextPhi), 0, (float)Math.Sin(nextPhi), 0);

                Vector4 leftBot = GetCylinderPoint(phi, false);
                Vector4 leftTop = GetCylinderPoint(phi, true);
                Vector4 rightBot = GetCylinderPoint(nextPhi, false);
                Vector4 rightTop = GetCylinderPoint(nextPhi, true);
                
                //Top triangle
                Vertex leftTopUp = new Vertex(leftTop, toUp);
                leftTopUp.TextureCoordinates = txt0;
                Vertex rightTopUp = new Vertex(rightTop, toUp);
                rightTopUp.TextureCoordinates = txt1;
                var triangleTop = new Triangle(rightTopUp, top, leftTopUp);
                newModelTriangles.Add(triangleTop);
                //Bot triangle
                Vertex leftBotDown = new Vertex(leftBot, toDown);
                leftBotDown.TextureCoordinates = txt0;
                Vertex rightBotDown = new Vertex(rightBot, toDown);
                rightBotDown.TextureCoordinates = txt1;
                var triangleBot = new Triangle(bot, rightBotDown, leftBotDown);
                newModelTriangles.Add(triangleBot);
                
                //Left and right triangles
                Vertex leftBotFront = new Vertex(leftBot, firstNormal);
                leftBotFront.TextureCoordinates = tx0;
                Vertex leftTopFront = new Vertex(leftTop, firstNormal);
                leftTopFront.TextureCoordinates = tx1;
                Vertex rightBotFront = new Vertex(rightBot, secondNormal);
                rightBotFront.TextureCoordinates = tx2;
                Vertex rightTopFront = new Vertex(rightTop, secondNormal);
                rightTopFront.TextureCoordinates = tx3;
                
                var triangleLeft = new Triangle(leftBotFront, rightBotFront, leftTopFront);
                var triangleRight = new Triangle(leftTopFront, rightBotFront, rightTopFront);
                newModelTriangles.Add(triangleLeft);
                newModelTriangles.Add(triangleRight);

                phi += phiStep;
            }

            ModelTriangles = newModelTriangles;
            UpdateWorldTriangles();
        }

        private Vector4 GetCylinderPoint(float phi, bool up)
        {
            return new Vector4(
                _radius * (float)Math.Cos(phi),
                up ? _height / 2 : -_height / 2,
                _radius * (float)Math.Sin(phi),
                1);
        }
    }
}