using System;
using System.Collections.Generic;
using ShapeDrafter.Models;

namespace ShapeDrafter.Figures
{
    public class Sphere : Figure
    {
        private float _radius;
        private int _psiDivision;
        private int _phiDivision;

        public Sphere(float radius, int psiDivision, int phiDivision)
        {
            _radius = radius;
            _psiDivision = psiDivision;
            _phiDivision = phiDivision;
            
            UpdateModelMatrix();
            UpdateModelTriangles();
        }

        public Sphere()
        {
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

        public int Psi
        {
            get => _psiDivision;
            set
            {
                _psiDivision = value;
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
            
            float psiStep = (float) Math.PI / _psiDivision;
            float phiStep = 2 * (float) Math.PI / _phiDivision;

            // Vertex top = new Vector4(0,_radius,0,1).ToSphereVertex();
            // Vertex bot = new Vector4(0,-_radius,0,1).ToSphereVertex();
            
            float phi = 0f;
            for (int i = 0; i < _phiDivision; i++)
            {
                float nextPhi = phi + phiStep;
                float texturePhi = i / (float)_phiDivision;
                float textureNextPhi = (i + 1) / (float)_phiDivision;
                float psi = psiStep;
                
                //Top Triangle
                float textNextPsi = 1 / (float)_psiDivision;
                Vector2F txt0 = new Vector2F(texturePhi,textNextPsi);
                Vector2F txt1 = new Vector2F(textureNextPhi,textNextPsi);
                Vector2F txt2 = new Vector2F(texturePhi,0);
                Vertex left = GetSpherePoint(phi, psi).ToSphereVertex();
                left.TextureCoordinates = txt0;
                Vertex right = GetSpherePoint(nextPhi, psi).ToSphereVertex();
                right.TextureCoordinates = txt1;
                Vertex top = GetSpherePoint(phi, 0).ToSphereVertex();
                top.TextureCoordinates = txt2;
                var triangleTop = new Triangle(left, right, top);
                newModelTriangles.Add(triangleTop);

                //Interim triangles
                for (int j = 0; j < _psiDivision - 2; j++)
                {
                    float nextPsi = psi + psiStep;
                    float texturePsi = (j + 1) / (float)_psiDivision;
                    float textureNextPsi = (j + 2) / (float)_psiDivision;
                    Vector2F tx0 = new Vector2F(texturePhi,texturePsi);
                    Vector2F tx1 = new Vector2F(texturePhi,textureNextPsi);
                    Vector2F tx2 = new Vector2F(textureNextPhi,texturePsi);
                    Vector2F tx3 = new Vector2F(textureNextPhi,textureNextPsi);
                    
                    Vertex leftBot = GetSpherePoint(phi, nextPsi).ToSphereVertex();
                    leftBot.TextureCoordinates = tx1;
                    Vertex leftTop = GetSpherePoint(phi, psi).ToSphereVertex();
                    leftTop.TextureCoordinates = tx0;
                    Vertex rightBot = GetSpherePoint(nextPhi, nextPsi).ToSphereVertex();
                    rightBot.TextureCoordinates = tx3;
                    Vertex rightTop = GetSpherePoint(nextPhi, psi).ToSphereVertex();
                    rightTop.TextureCoordinates = tx2;
                    var triangleLeft = new Triangle(leftBot, rightBot, leftTop);
                    var triangleRight = new Triangle(rightBot, rightTop, leftTop);
                    newModelTriangles.Add(triangleLeft);
                    newModelTriangles.Add(triangleRight);

                    psi += psiStep;
                }
                //Bottom Triangle
                float textLastPsi = (_psiDivision - 1) / (float) _psiDivision;
                Vector2F txb0 = new Vector2F(texturePhi,textLastPsi);
                Vector2F txb1 = new Vector2F(textureNextPhi,textLastPsi);
                Vector2F txb2 = new Vector2F(texturePhi,1);
                left = GetSpherePoint(phi, psi).ToSphereVertex();
                left.TextureCoordinates = txb0;
                right = GetSpherePoint(nextPhi, psi).ToSphereVertex();
                right.TextureCoordinates = txb1;
                Vertex bot = GetSpherePoint(phi, (float) Math.PI).ToSphereVertex();
                bot.TextureCoordinates = txb2;
                var triangleBot = new Triangle(bot, right, left);
                newModelTriangles.Add(triangleBot);

                phi += phiStep;
            }

            ModelTriangles = newModelTriangles;
            UpdateWorldTriangles();
        }

        private Vector4 GetSpherePoint(float phi, float psi)
        {
            return new Vector4(
                _radius * (float)(Math.Sin(psi) * Math.Cos(phi)),
                _radius * (float)Math.Cos(psi),
                _radius * (float)(Math.Sin(psi) * Math.Sin(phi)),
                1);
        }
    }
}