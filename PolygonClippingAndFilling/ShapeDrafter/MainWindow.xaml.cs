using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ShapeDrafter.Graphics;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Button[] _buttons = new Button[5];
        private int _currentButton;
        private WriteableBitmap _wb;
        public static MainWindow Instance;
        private bool _running = false;
        private List<ColorPolygon> _generated = new List<ColorPolygon>();
        private static Random Random = new Random();
        private Texture MainTexture = new Texture();
        private Vector3D[,] BumpMap;
        private Color _lightColor = Color.Yellow;
        private Vector3D _lightPos = new Vector3D(400,300,100);
        private bool _useColors = true;
        public double[,] dotProducts = new double[Texture.Width,Texture.Height];

        public void UpdateDotProducts()
        {
            for (int i = 0; i < Texture.Width; i++)
            {
                for (int j = 0; j < Texture.Height; j++)
                {
                    var pixelPos = new Vector3D((double) i , (double) j , 0);
                    var LVector = _lightPos - pixelPos;
                    var NVector = new Vector3D(0, 0, 1);
                    if (BumpMap != null)
                    {
                        var D = BumpMap[i, j];
                        NVector += D;
                    }
                    dotProducts[i,j] = DotProduct(LVector,NVector);
                } 
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            SetupButtonTab();

            _width = (int) PaintSurface.Width;
            _height = (int) PaintSurface.Height;
            _sourceRect = new Int32Rect(0, 0, _width, _height);
            SetupBitmap();
            _sourceBuffer = new byte[_width * _height * (_wb.Format.BitsPerPixel / 8)];
            ClearScreen();
            Instance = this;
            RedrawAll();

            var points1 = new System.Windows.Point[]{new System.Windows.Point(1, 1),new System.Windows.Point(1,50),new System.Windows.Point(50,1)};
            var points2 = new System.Windows.Point[]{new System.Windows.Point(10, 10),new System.Windows.Point(10,50),new System.Windows.Point(50,10)};
            var clip = SutherlandHodgman.GetIntersectedPolygon(points1, points2);

            List<Point> pointz = new List<Point>()
            {
                new Point(3,0),
                new Point(0,3),
                new Point(3,6),
                new Point(6,3)
            };
            var filling = ScanLine.PolygonFilling(pointz,out var colorTab,true);
            
            UpdateDotProducts();
            StartMainLoop();
        }

        private void SetupBitmap()
        {
            // ReSharper disable once LocalizableElement
            _wb = new WriteableBitmap(
                _width,
                _height,
                96,
                96,
                PixelFormats.Bgra32,
                null);

            PaintSurface.Source = _wb;
        }

        //Stuff with buttons
        private void SetupButtonTab()
        {
            _buttons[0] = Button0;
            _buttons[1] = Button1;
            _buttons[2] = Button2;
            _buttons[3] = Button3;
            _buttons[4] = Button4;
            
            DisableButtons(0);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var numOfButton = int.Parse(((Button)sender).Tag.ToString());
            _currentButton = numOfButton;
            
            if (_currentButton != 0)
            {
                ClearUnfinishedPolygon();
                RedrawAll();
            }
            
            if (numOfButton == 100)
            {
                ClearScreen();
            }
            
            DisableButtons(numOfButton);
        }

        private void DisableButtons(int numOfSelectedButton)
        {
            foreach (var button in _buttons)
            {
                button.Opacity = 0.5;
            }

            var selectedButton = _buttons[numOfSelectedButton];
            selectedButton.Opacity = 1;
        }

        private void OnMouseClicked(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(PaintSurface);
            var intPoint = new Point((int) point.X, (int) point.Y);

            Console.WriteLine(point);
            switch (_currentButton)
            {
                case 0:
                    DrawPolygon(intPoint);
                    return;
                case 1:    
                    DeleteShape(intPoint);
                    RedrawAll();
                    return;
                case 2:
                    ShiftStart(intPoint);
                    return;
                case 3:
                    ShapeShiftStart(intPoint);
                    return; 
                case 4:
                    Canvas.SetLeft(BulbImage,point.X);
                    Canvas.SetTop(BulbImage,point.Y);
                    _lightPos.X = point.X;
                    _lightPos.Y = point.Y;
                    UpdateDotProducts();
                    RedrawAll();
                    return;
                case 9:
                    DeleteShape(intPoint);
                    RedrawAll();
                    return;
            }
        }
        private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (e.NewValue != null)
            {
                var col = e.NewValue.Value;
                _lightColor = Color.FromArgb(col.A, col.R, col.G, col.B);
            }
            RedrawAll();
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var point = e.GetPosition(PaintSurface);
                var intPoint = new Point((int) point.X, (int) point.Y);
                
                if (_currentButton == 2 || _currentButton == 3)
                {
                    ShiftMove(intPoint);
                    RedrawAll();
                }
            }
//            RedrawAll();
        }

        private void OnMouseUnclicked(object sender, MouseButtonEventArgs e)
        {
            if (_currentButton == 2 || _currentButton == 3)
            {
                ShiftEnd();
            }
        }

        private void ProgressBar_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ProgressBar.Value = e.GetPosition(ProgressBar).X;
            _lightPos.Z = ProgressBar.Value * 10;
            UpdateDotProducts();
            RedrawAll();
            
        }

        private void ButtonStart_OnClick(object sender, RoutedEventArgs e)
        {
            _running = !_running;
            ButtonStart.Content = _running ? "STOP" : "START";
        }

        private void ButtonGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            var polygon = PolygonGenerator.GeneratePolygon(0,Random.Next(390), 120, 120);
            _generated.Add(polygon);
            RedrawAll();
        }

        private bool _firstcheck;
        private void RadioColorsChecked(object sender, RoutedEventArgs e)
        {
            _useColors = true;
            if (!_firstcheck)
            {
                _firstcheck = true;
                return;
            }
            RedrawAll();
        }
        private void RadioTexturesChecked(object sender, RoutedEventArgs e)
        {
            _useColors = false;
            RedrawAll();
        }

        private void LoadTextureClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Load Texture file";
            ofd.Filter = "jpg|*.jpg|png|*.png";
            if (ofd.ShowDialog().HasValue)
            {
                if (ofd.FileName == "") return;
                var bitmap = new Bitmap(ofd.FileName);
                MainTexture = new Texture(bitmap);
                Console.WriteLine(ofd.FileName);
            }
            RedrawAll();
        }

        private void LoadBumpmapClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Load Bumpmap file";
            ofd.Filter = "jpg|*.jpg|png|*.png";
            if (ofd.ShowDialog().HasValue)
            {
                if (ofd.FileName == "") return;
                var bitmap = new Bitmap(ofd.FileName);
                CreateBumpMap(bitmap);
            }
            UpdateDotProducts();
            RedrawAll();
        }
    }
}
