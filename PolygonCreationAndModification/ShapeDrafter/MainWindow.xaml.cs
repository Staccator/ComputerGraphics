using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace ShapeDrafter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Button[] _buttons = new Button[10];
        private int _currentButton;
        private WriteableBitmap _wb;
        public static MainWindow instance;

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
            instance = this;

//            var rect = new Rectangle();
//            rect.Width = 50;
//            rect.Height = 50;
//            rect.Fill = new SolidColorBrush(Colors.Crimson);
//            Canvas.SetLeft(rect,100);
//            Canvas.SetTop(rect,200);
//            Canvas.Children.Add(rect);
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
            _buttons[0] = Button1;
            _buttons[1] = Button2;
            _buttons[2] = Button3;
            _buttons[3] = Button4;
            _buttons[4] = Button5;
            _buttons[5] = Button6;
            _buttons[6] = Button7;
            _buttons[7] = Button8;
            _buttons[8] = Button9;
            _buttons[9] = Button10;
            
            DisableButtons(0);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var numOfButton = int.Parse(((Button) sender).Tag.ToString()) - 1;
            _currentButton = numOfButton;
            
            if (_currentButton != 0)
            {
                ClearUnfinishedPolygon();
                RedrawAll();
            }
            
            if (numOfButton == 8)
            {
                ClearScreen();
            }

            if (numOfButton != 6 || numOfButton != 7)
            {
                _firstParallel = true;
            }
            
            DisableButtons(numOfButton);
        }

        private void DisableButtons(int numOfSelectedButton)
        {
            for (var i = 0; i < _buttons.Length; i++) _buttons[i].Opacity = 0.5;

            var selectedButton = _buttons[numOfSelectedButton];
            selectedButton.Opacity = 1;
        }

        private void OnMouseClicked(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(PaintSurface);
            var intPoint = new Point((int) point.X, (int) point.Y);

            switch (_currentButton)
            {
                case 0:
                    DrawPolygon(intPoint);
                    return;
                case 1:
                    DrawCircle(intPoint);
                    return;
                case 2:
                    Delete(intPoint);
                    return;
                case 3:
                    AddVertex(intPoint);
                    return;
                case 4:
                    ShiftStart(intPoint);
                    return;
                case 5:
                    ShapeShiftStart(intPoint);
                    return;
                case 6:
                    Parallelize(intPoint);
                    return;
                case 7:
                    Parallelize(intPoint,false);
                    return;
                case 9:
                    DeleteShape(intPoint);
                    RedrawAll();
                    return;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var point = e.GetPosition(PaintSurface);
                var intPoint = new Point((int) point.X, (int) point.Y);
                
                if (_currentButton == 4 || _currentButton == 5)
                {
                    ShiftMove(intPoint);
                }
            }
        }

        private void OnMouseUnclicked(object sender, MouseButtonEventArgs e)
        {
            
            if (_currentButton == 4 || _currentButton == 5)
            {
                ShiftEnd();
            }
        }

        private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (e.NewValue != null)
            {
                var col = e.NewValue.Value;
                _defaultLineColor = Color.FromArgb(col.A, col.R, col.G, col.B);
            }
        }
    }
}
