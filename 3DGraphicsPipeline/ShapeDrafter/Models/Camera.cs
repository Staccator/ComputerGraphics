using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ShapeDrafter.MathOperations;

namespace ShapeDrafter.Models
{
    public partial class Camera
    {
        public static float ScreenWidth = 200;
        public static float ScreenHeight = 100;
        private float _near;
        private float _far;
        private float _fov;
        private Vector3 _p = new Vector3(0, 0, 20);
        private Vector3 _f = new Vector3(0, 0, 1);
        private static Vector3 _uWorld = new Vector3(0, 1, 0);
        [XmlIgnore]
        public Matrix ViewMatrix;
        [XmlIgnore]
        public Matrix ProjectionMatrix;

        public Camera()
        {
        }

        public float Near
        {
            get => _near;
            set
            {
                _near = value;
                RefreshProjectionMatrix();
            }
        }

        public float Far
        {
            get => _far;
            set
            {
                _far = value; 
                RefreshProjectionMatrix();
            }
        }

        public float Fov
        {
            get => _fov;
            set
            {
                _fov = value;
                RefreshProjectionMatrix();
            }
        }

        public Vector3 P
        {
            get => _p;
            set
            {
                _p = value;
                RefreshViewMatrix();
            }
        }

        public Vector3 F
        {
            get => _f;
            set
            {
                _f = value;
                _f.Normalize();
                RefreshViewMatrix();
            }
        }


        public Camera(float near, float far, float fov)
        {
            _near = near;
            _far = far;
            _fov = fov;
            RefreshProjectionMatrix();
            RefreshViewMatrix();
        }

        public void RefreshProjectionMatrix()
        {
            float ctgfov = 1/(float)Math.Tan(_fov/2);
            float aspect = ScreenWidth / ScreenHeight;
            
            ProjectionMatrix =  new Matrix(new[]
            {
                ctgfov / aspect, 0, 0, 0,
                0, ctgfov, 0, 0,
                0, 0, -(_far + _near) / (_far - _near), -2 *_far * _near / (_far - _near),
                0, 0, -1, 0,
            });
        }
        private void RefreshViewMatrix()
        {
            Vector3 r = _uWorld.CrossProduct(F);
            r = r.Normalized();
            Vector3 u = F.CrossProduct(r);
            u = u.Normalized();

            var matrix = new Matrix(new[]
            {
                r.X, r.Y, r.Z, 0,
                u.X, u.Y, u.Z, 0,
                F.X, F.Y, F.Z, 0,
                0, 0, 0, 1,
            });

            ViewMatrix = matrix * Matrix.TranslationMatrix(-P);
        }

        public void RotateHorizontally(float rotateDiff)
        {
            Vector3 r = _uWorld.CrossProduct(F).Normalized();
            // var rotationMatrix = Matrix.RotateYMatrix(rotateDiff);
            // var newF = rotationMatrix * _f;
            var newF = Renderer.Interpolate(_f, r, -rotateDiff).Normalized();
            F = newF;
        }

        public void RotateVertically(float rotateDiff)
        {
            Vector3 r = _uWorld.CrossProduct(F).Normalized();
            Vector3 u = F.CrossProduct(r).Normalized();
            // var rotationMatrix = Matrix.RotateXMatrix(rotateDiff);
            // var newF = rotationMatrix * _f;
            var newF = Renderer.Interpolate(_f, u, -rotateDiff).Normalized();
            F = newF;
        }

        public void MoveRight()
        {
            Vector3 r = _uWorld.CrossProduct(F).Normalized();
            P = P + r;
        }
        public void MoveLeft()
        {
            Vector3 r = _uWorld.CrossProduct(F).Normalized();
            P = P - r;
        }
        public void MoveFront()
        {
            P = P - F;
        }
        public void MoveBack()
        {
            P = P + F;
        }
        public void MoveUp()
        {
            Vector3 r = _uWorld.CrossProduct(F).Normalized();
            Vector3 u = F.CrossProduct(r).Normalized();
            P = P + u;
        }
        public void MoveDown()
        {
            Vector3 r = _uWorld.CrossProduct(F).Normalized();
            Vector3 u = F.CrossProduct(r).Normalized();
            P = P - u;
        }
    }
}
