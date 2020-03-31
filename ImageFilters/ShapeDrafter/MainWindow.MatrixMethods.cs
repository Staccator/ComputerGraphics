using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private double[,] GetMatrix()
        {
            double[,] result = new double[3,3];
            switch (_currentMatrix)
            {
                case 0:
                    result[0, 0] = 0;
                    result[1, 0] = 0;
                    result[2, 0] = 0;
                    result[0, 1] = 0;
                    result[1, 1] = 1;
                    result[2, 1] = 0;
                    result[0, 2] = 0;
                    result[1, 2] = 0;
                    result[2, 2] = 0;
                    break;
                case 1:
                    result[0, 0] = 1;
                    result[1, 0] = 2;
                    result[2, 0] = 1;
                    result[0, 1] = 2;
                    result[1, 1] = 4;
                    result[2, 1] = 2;
                    result[0, 2] = 1;
                    result[1, 2] = 2;
                    result[2, 2] = 1;
                    break;
                case 2:
                    result[0, 0] = 0;
                    result[1, 0] = -1;
                    result[2, 0] = 0;
                    result[0, 1] = -1;
                    result[1, 1] = 5;
                    result[2, 1] = -1;
                    result[0, 2] = 0;
                    result[1, 2] = -1;
                    result[2, 2] = 0;
                    break;
                case 3:
                    result[0, 0] = -1;
                    result[1, 0] = -1;
                    result[2, 0] = 0;
                    result[0, 1] = -1;
                    result[1, 1] = 1;
                    result[2, 1] = 1;
                    result[0, 2] = 0;
                    result[1, 2] = 1;
                    result[2, 2] = 1;
                    break;
                case 4:
                    result[0, 0] = -1;
                    result[1, 0] = -2;
                    result[2, 0] = -1;
                    result[0, 1] = 0;
                    result[1, 1] = 0;
                    result[2, 1] = 0;
                    result[0, 2] = -1;
                    result[1, 2] = -2;
                    result[2, 2] = -1;
                    break;
                case 5:
                    return _ownMatrix;
            }

            return result;
        }

        private void MatrixMethodOnChecked(object sender, RoutedEventArgs e)
        {
            var tag = (sender as RadioButton).Tag;
            if (tag != null) _currentMatrix = int.Parse((string) tag);
        }

        private void CellValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string tag = (string)(sender as DoubleUpDown).Tag;
            var values = tag.Split('x');
            int column = int.Parse(values[0]);
            int row = int.Parse(values[1]);
            Console.WriteLine(column + " :" + row);
            _ownMatrix[column, row] = (double) e.NewValue;

//            int cell = int.Parse((string) (sender as DoubleUpDown).Tag);
//            double cellValue = (double)e.NewValue;
//            int x = cell % 3;
//            int y = cell / 3;
//            _ownMatrix[x, y] = cellValue;
        }

        private void UserValuesChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is null) return;
            
            int tag = int.Parse((string) (sender as DoubleUpDown).Tag);
            double value = (double)e.NewValue;
            if (tag == 0)
            {
                _shift = value;
            }
            else
            {
                _divider = value;
            }
        }
    }
}