using System;
using System.Collections.Generic;
using ShapeDrafter.Models;

namespace ShapeDrafter.Figures
{
    public class Cube : Figure
    {
        private float _xLength;
        private float _yLength;
        private float _zLength;

        public Cube(float xLength, float yLength, float zLength)
        {
            _xLength = xLength;
            _yLength = yLength;
            _zLength = zLength;
            
            UpdateModelMatrix();
            UpdateModelTriangles();
        }

        public Cube()
        {
        }

        public float XLength
        {
            get => _xLength;
            set
            {
                _xLength = value;
                UpdateModelTriangles();
            }
        }

        public float YLength
        {
            get => _yLength;
            set
            {
                _yLength = value;
                UpdateModelTriangles();
            }
        }

        public float ZLength
        {
            get => _zLength;
            set
            {
                _zLength = value;
                UpdateModelTriangles();
            }
        }

        public sealed override void UpdateModelTriangles()
        {
            float xdiff = _xLength / 2;
            float ydiff = _yLength / 2;
            float zdiff = _zLength / 2;
            
            //Positions
            Vector4 frontLeftTop = new Vector4(-xdiff, ydiff, zdiff,1);
            Vector4 frontLeftBot = new Vector4(-xdiff, -ydiff, zdiff,1);
            Vector4 frontRightTop = new Vector4(xdiff, ydiff, zdiff,1);
            Vector4 frontRightBot = new Vector4(xdiff, -ydiff, zdiff,1);
            Vector4 backLeftTop  = new Vector4(-xdiff, ydiff, -zdiff,1);
            Vector4 backLeftBot  = new Vector4(-xdiff, -ydiff, -zdiff,1);
            Vector4 backRightTop = new Vector4(xdiff, ydiff, -zdiff,1);
            Vector4 backRightBot = new Vector4(xdiff, -ydiff, -zdiff,1);
            //Normals
            Vector4 toFront = new Vector4(0,0,1,0);
            Vector4 toBack = new Vector4(0,0,-1,0);
            Vector4 toLeft = new Vector4(-1,0,0,0);
            Vector4 toRight = new Vector4(1,0,0,0);
            Vector4 toUp = new Vector4(0,1,0,0);
            Vector4 toDown = new Vector4(0,-1,0,0);
           
            Vector2F tx1 = new Vector2F(0,0);
            Vector2F tx2 = new Vector2F(1,0);
            Vector2F tx3 = new Vector2F(0,1);
            Vector2F tx4 = new Vector2F(1,1);
            //Triangles Front
            Vertex v0 = new Vertex(frontLeftBot, toFront);
            v0.TextureCoordinates = tx1;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            Vertex v1 = new Vertex(frontLeftTop, toFront);
            v1.TextureCoordinates = tx2;
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            Vertex v2 = new Vertex(frontRightBot, toFront);
            v2.TextureCoordinates = tx3;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri0 = new Triangle(v0, v1, v2);
            
            v0 = new Vertex(frontRightBot, toFront);
            v0.TextureCoordinates = tx3;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            v1 = new Vertex(frontLeftTop, toFront);
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            v1.TextureCoordinates = tx2;
            v2 = new Vertex(frontRightTop, toFront);
            v2.TextureCoordinates = tx4;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri1 = new Triangle(v0, v1, v2);
            
            //Triangles Right
            v0 = new Vertex(frontRightBot, toRight);
            v0.TextureCoordinates = tx1;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            v1 = new Vertex(frontRightTop, toRight);
            v1.TextureCoordinates = tx2;
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            v2 = new Vertex(backRightBot, toRight);
            v2.TextureCoordinates = tx3;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri2 = new Triangle(v0, v1, v2);
            
            v0 = new Vertex(backRightBot, toRight);
            v0.TextureCoordinates = tx3;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            v1 = new Vertex(frontRightTop, toRight);
            v1.TextureCoordinates = tx2;
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            v2 = new Vertex(backRightTop, toRight);
            v2.TextureCoordinates = tx4;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri3 = new Triangle(v0, v1, v2);
            
            //Triangles Back
            v0 = new Vertex(backRightBot, toBack);
            v0.TextureCoordinates = tx1;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            v1 = new Vertex(backRightTop, toBack);
            v1.TextureCoordinates = tx2;
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            v2 = new Vertex(backLeftBot, toBack);
            v2.TextureCoordinates = tx3;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri4 = new Triangle(v0, v1, v2);
            
            v0 = new Vertex(backLeftBot, toBack);
            v0.TextureCoordinates = tx3;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            v1 = new Vertex(backRightTop, toBack);
            v1.TextureCoordinates = tx2;
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            v2 = new Vertex(backLeftTop, toBack);
            v2.TextureCoordinates = tx4;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri5 = new Triangle(v0, v1, v2);
            
            //Triangles Left
            v0 = new Vertex(backLeftBot, toLeft);
            v0.TextureCoordinates = tx1;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            v1 = new Vertex(backLeftTop, toLeft);
            v1.TextureCoordinates = tx2;
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            v2 = new Vertex(frontLeftBot, toLeft);
            v2.TextureCoordinates = tx3;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri6 = new Triangle(v0, v1, v2);
            
            v0 = new Vertex(frontLeftBot, toLeft);
            v0.TextureCoordinates = tx3;
            v0.Tangent = toUp;
            v0.BiNormal = v0.Tangent.CrossProduct(v0.Normal);
            v1 = new Vertex(backLeftTop, toLeft);
            v1.TextureCoordinates = tx2;
            v1.Tangent = toUp;
            v1.BiNormal = v1.Tangent.CrossProduct(v1.Normal);
            v2 = new Vertex(frontLeftTop, toLeft);
            v2.TextureCoordinates = tx4;
            v2.Tangent = toUp;
            v2.BiNormal = v2.Tangent.CrossProduct(v2.Normal);
            Triangle tri7 = new Triangle(v0, v1, v2);
            
            //Triangles Down
            v0 = new Vertex(backLeftBot, toDown);
            v0.TextureCoordinates = tx1;
            v1 = new Vertex(frontLeftBot, toDown);
            v1.TextureCoordinates = tx2;
            v2 = new Vertex(backRightBot, toDown);
            v2.TextureCoordinates = tx3;
            Triangle tri8 = new Triangle(v0, v1, v2);
            
            v0 = new Vertex(backRightBot, toDown);
            v0.TextureCoordinates = tx3;
            v1 = new Vertex(frontLeftBot, toDown);
            v1.TextureCoordinates = tx2;
            v2 = new Vertex(frontRightBot, toDown);
            v2.TextureCoordinates = tx4;
            Triangle tri9 = new Triangle(v0, v1, v2);
            
            //Triangles Up
            v0 = new Vertex(frontLeftTop, toUp);
            v0.TextureCoordinates = tx1;
            v1 = new Vertex(backLeftTop, toUp);
            v1.TextureCoordinates = tx2;
            v2 = new Vertex(frontRightTop, toUp);
            v2.TextureCoordinates = tx3;
            Triangle tri10 = new Triangle(v0, v1, v2);
            
            v0 = new Vertex(frontRightTop, toUp);
            v0.TextureCoordinates = tx3;
            v1 = new Vertex(backLeftTop, toUp);
            v1.TextureCoordinates = tx2;
            v2 = new Vertex(backRightTop, toUp);
            v2.TextureCoordinates = tx4;
            Triangle tri11 = new Triangle(v0, v1, v2);
            
            var newModelTriangles = new List<Triangle>();
            newModelTriangles.Add(tri0);
            newModelTriangles.Add(tri1);
            newModelTriangles.Add(tri2);
            newModelTriangles.Add(tri3);
            newModelTriangles.Add(tri4);
            newModelTriangles.Add(tri5);
            newModelTriangles.Add(tri6);
            newModelTriangles.Add(tri7);
            newModelTriangles.Add(tri8);
            newModelTriangles.Add(tri9);
            newModelTriangles.Add(tri10);
            newModelTriangles.Add(tri11);

            ModelTriangles = newModelTriangles;
            UpdateWorldTriangles();
        }
    }
}