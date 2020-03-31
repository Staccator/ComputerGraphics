using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShapeDrafter.Graphics;
using ShapeDrafter.Models;
using Xceed.Wpf.Toolkit;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;

namespace ShapeDrafter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance;
        private WriteableBitmap _wb;
        private Texture _mainTexture;
        private int _radius;
        private Point _circleCenter = new Point(100,100);
        private int _currentMatrix;
        private double[,] _ownMatrix = new double[3,3];
        private double _shift;
        private double _divider;
        private List<Point> _polygonFilling = new List<Point>();

        public MainWindow()
        {
            InitializeComponent();

            _width = (int) PaintSurface.Width;
            _height = (int) PaintSurface.Height;
            _sourceRect = new Int32Rect(0, 0, _width, _height);
            SetupBitmap();
            _sourceBuffer = new byte[_width * _height * (_wb.Format.BitsPerPixel / 8)];
            ClearScreen();
            Instance = this;

            var pointz = new List<Point>
            {
                new Point(3, 0),
                new Point(0, 3),
                new Point(3, 6),
                new Point(6, 3)
            };
            var filling = ScanLine.PolygonFilling(pointz, out var colorTab, true);
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

        private void OnMouseClicked(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(PaintSurface);
            var intPoint = new Point((int) point.X, (int) point.Y);

            //Console.WriteLine(point);
            switch (_currentAreaMethod)
            {
                case 0:
                    ExecuteFilter();
                    return;
                case 1:
                    _circleCenter = intPoint;
                    return;
                case 2:
                    DrawPolygon(intPoint);
                    return;
            }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var point = e.GetPosition(PaintSurface);
                var intPoint = new Point((int) point.X, (int) point.Y);

                if (_currentAreaMethod == 1)
                {
                    _circleCenter = intPoint;
                    ExecuteFilter(_circleCenter);
                }
            }
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
                _mainTexture = new Texture(bitmap);
                Console.WriteLine(ofd.FileName);
            }

            ClearScreen();
            RedrawTexture();
//            ExecuteClicked(null,null);
            UpdateHistograms();
        }

        private void UpdateHistograms()
        {
            _mainTexture.GetHistograms(out Bitmap redColor, out Bitmap greenColor, out Bitmap blueColor);
            RedHistogram.Source = BitmapToImageSource(redColor);
            GreenHistogram.Source = BitmapToImageSource(greenColor);
            BlueHistogram.Source = BitmapToImageSource(blueColor);
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _radius = (int) (e.NewValue * 30);
            RadiusLabel.Content = _radius.ToString();
        }

        private void ExecuteFilter(Point? center = null)
        {
            if (_mainTexture == null)
                return;
            if (_started)
            {
                ClearUnfinishedPolygon();
            }
            
//            RedrawImage();
            double[,] matrix = GetMatrix();
            List<Point> pointsToChange = GetArea(center);
            Color[] colors;
            if ((bool) Divider.IsChecked)
            {
                colors = _mainTexture.TransformPoints(pointsToChange, matrix, _currentMatrix, _divider);
            }
            else
            {
                colors = _mainTexture.TransformPoints(pointsToChange, matrix, _currentMatrix);
            }

            DrawPoints(pointsToChange, Color.Black, colors);
            UpdateHistograms();
            CommitDraw();
        }

        private void RedrawImage()
        {
            _mainTexture.GetAllPixels(out var points, out var colors);
            DrawPoints(points,Color.Black,colors.ToArray());
            CommitDraw();
        }

        private void ExecuteClearClick(object sender, RoutedEventArgs e)
        {
            if (_mainTexture == null)
            {
                return;
            }
            RedrawImage();
            _mainTexture.ResetTempTab();
            UpdateHistograms();
        }

        private void MatrixSizeClick(object sender, RoutedEventArgs e)
        {
            int width = MatrixWidth.Value.Value;
            int height = MatrixHeight.Value.Value;
            Console.WriteLine(width + "   " + height);
            Grid.ColumnDefinitions.Clear();
            Grid.RowDefinitions.Clear();
            for (int i = 0; i < width; i++)
            {
                Grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < height; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition());
            }

            Grid.Children.Clear();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    DoubleUpDown upDown = new DoubleUpDown();
                    upDown.Value = 0;
                    upDown.Tag = $"{i}x{j}";
                    upDown.ValueChanged += CellValueChanged;
                    Grid.SetColumn(upDown,i);
                    Grid.SetRow(upDown,j);
                    Grid.Children.Add(upDown);
                }
            }
            
            _ownMatrix = new double[width, height];
        }
    }
}