using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ShapeDrafter.Graphics;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using System.Windows.Media;
using ShapeDrafter.Figures;
using ShapeDrafter.MathOperations;
using ShapeDrafter.Models;
using Geometry = ShapeDrafter.Graphics.Geometry;
using Matrix = ShapeDrafter.MathOperations.Matrix;

namespace ShapeDrafter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private WriteableBitmap _wb;
        public static MainWindow Instance;

        private Vector3 _lightPos = new Vector3(400,300,100);
        private Color _lightColor = Color.Yellow;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            
            _width = (int) PaintSurface.Width;
            _height = (int) PaintSurface.Height;
            UpdateWidthHeight();
            
            _sourceBuffer = CreateNewBuffer();
            ClearScreen();
            CommitDraw();

            // Test();
            Startup();
            //UpdateDotProducts();
            // StartMainLoop();
        }

        private void Startup()
        {
            CreateCameraClick(null,null);
            CreateLightClick(null,null);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Console.WriteLine(sizeInfo.NewSize.Width + " | " + sizeInfo.NewSize.Height);
            _width = (int) sizeInfo.NewSize.Width * 2 / 5;
            _height = (int) sizeInfo.NewSize.Height * 1 / 2;
            Console.WriteLine(_width + " " + _height);
            
            PaintSurface.Width = _width;
            PaintSurface.Height = _height;
            UpdateWidthHeight();

            UpdateScene();
        }

        private void UpdateWidthHeight()
        {
            Camera.ScreenWidth = _width;
            Camera.ScreenHeight = _height;
            Vertex.Width = _width;
            Vertex.Height = _height;

            SetupBitmap();
            _sourceRect = new Int32Rect(0, 0, _width, _height);
            // PaintSurface.UpdateLayout();
            // PaintSurface.InvalidateVisual();
        }
        
        private void Test()
        {
            // var v1 = new Vertex(new Vector4(1f,2f,3f,4f),new Vector4(4f,3f,2f,1f), new ScreenPosition(5,5,0),5, new Vector2F(1,0));
            // var v2 = new Vertex(new Vector4(4f,5f,6f,7f),new Vector4(7f,6f,5f,4f), new ScreenPosition(5,1,10),10,new Vector2F(1,1));
            // var v3 = new Vertex(new Vector4(0f,0f,0f,0f),new Vector4(0f,0f,0f,0f), new ScreenPosition(10,5,20),20, new Vector2F(0,1));
            // var triangle = new Triangle(v1, v2, v3);
            // var vectors = Renderer.Rasterize(triangle);

            bool result = Geometry.IsTriangleClockwise(new Vector2F(2,1), new Vector2F(1,5),new Vector2F(4,3));
            
            var array = new float[]
            {
                1, -1, 3, 4,
                5, -3, -1, 8,
                4, 0, -1, -2,
                2, 13, -4, -5,
            };
            var matrix = new Matrix(array);
            var res = matrix.Determinant();
            var inverse = matrix.Inversed();
        }
    }
}
