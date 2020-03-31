using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Point = System.Drawing.Point;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private int _currentAreaMethod;

        private List<Point> GetArea(Point? circcenter = null)
        {
            var result = new List<Point>();
            switch (_currentAreaMethod)
            {
                case 0:
                    _mainTexture.GetAllPixels(out var points, out var colors);
                    return points;
                case 1:
                    var center = circcenter.Value;
                    for (int i = center.X - _radius; i <= center.X + _radius; i++)
                    for (int j = center.Y - _radius; j <= center.Y + _radius; j++)
                    {
                        if (i < 0 || i >= _mainTexture.Width || j < 0 || j >= _mainTexture.Height)
                        {
                            continue;
                        }

                        int diffx = center.X - i;
                        int diffy = center.Y - j;
                        double length = Math.Sqrt(diffx * diffx + diffy * diffy);
                        if (length <= _radius)
                        {
                            result.Add(new Point(i,j));
                        }
                    }

                    break;
                case 2:
                    return _polygonFilling;
                    
            }

            return result;
        }

        private void AreaMethodOnChecked(object sender, RoutedEventArgs e)
        {
            var tag = (sender as RadioButton).Tag;
            if (tag != null) _currentAreaMethod = int.Parse((string) tag);
            Console.WriteLine(_currentAreaMethod);
            Console.WriteLine(_currentMatrix);
        }
    }
}