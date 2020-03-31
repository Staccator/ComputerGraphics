using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ShapeDrafter.MathOperations;
using ShapeDrafter.Models;

namespace ShapeDrafter.Figures
{
    [XmlInclude(typeof(Cube))]
    [XmlInclude(typeof(Sphere))]
    [XmlInclude(typeof(Cylinder))]
    [XmlInclude(typeof(Cone))]
    public abstract class Figure
    {
        private Vector3 _translation;
        private Vector3 _scale = new Vector3(3f,3f,3f);
        private Vector3 _rotation;
        private Matrix _modelMatrix;
        private Matrix _normalMatrix;
        [XmlIgnore]
        public List<Triangle> ModelTriangles = new List<Triangle>();
        [XmlIgnore]
        public List<Triangle> WorldTriangles = new List<Triangle>();

        public Vector3 Translation
        {
            get => _translation;
            set
            {
                _translation = value;
                UpdateModelMatrix();
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateModelMatrix();
            }
        }

        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                UpdateModelMatrix();
            }
        }

        protected void UpdateModelMatrix()
        {
            _modelMatrix = Matrix.TranslationMatrix(_translation) * Matrix.RotateXMatrix(_rotation.X) * Matrix.RotateYMatrix(_rotation.Y) * Matrix.RotateZMatrix(_rotation.Z) * Matrix.ScaleMatrix(_scale);
            _normalMatrix = _modelMatrix.Transposed().Inversed();
            UpdateWorldTriangles();
        }

        public abstract void UpdateModelTriangles();

        public void UpdateWorldTriangles()
        {
            WorldTriangles = ModelTriangles.Select(t => t.ApplyModelMatrix(_modelMatrix, _normalMatrix)).ToList();
        }
    }
}