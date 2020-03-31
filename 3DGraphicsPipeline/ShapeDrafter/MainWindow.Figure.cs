using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShapeDrafter.Figures;
using ShapeDrafter.Models;
using Xceed.Wpf.Toolkit;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private Dictionary<Button, Figure> _buttonFigures = new Dictionary<Button, Figure>();
        private Button _selectedFigureButton = null;

        private void DeleteShapeClick(object sender, RoutedEventArgs e)
        {
            WrapPanel.Children.Remove(_selectedFigureButton);
            _buttonFigures.Remove(_selectedFigureButton);
            _selectedFigureButton = null;
            
            UpdateScene();
        }
        private void CreateShapeClick(object sender, RoutedEventArgs e)
        {
            ImageSource imageSource = null;
            Figure figure = null;

            if (sender is Figure fig)
            {
                figure = fig;
                switch (fig)
                {
                    case Cube cube:
                        imageSource = new BitmapImage(new Uri("Resources/cube.png", UriKind.Relative));
                        break;
                    case Sphere sphere:
                        imageSource = new BitmapImage(new Uri("Resources/sphere.png", UriKind.Relative));
                        break;
                    case Cylinder cylinder:
                        imageSource = new BitmapImage(new Uri("Resources/cylinder.png", UriKind.Relative));
                        break;
                    case Cone cone:
                        imageSource = new BitmapImage(new Uri("Resources/cone.png", UriKind.Relative));
                        break;
                }
            }
            else
            {
                int tag = int.Parse((string) ((Button) sender).Tag);
                switch (tag)
                {
                    case 0:
                        imageSource = new BitmapImage(new Uri("Resources/cube.png", UriKind.Relative));
                        figure = new Cube(2f, 2f, 2f);
                        break;
                    case 1:
                        imageSource = new BitmapImage(new Uri("Resources/sphere.png", UriKind.Relative));
                        figure = new Sphere(1f,6,8);
                        break;
                    case 2:
                        imageSource = new BitmapImage(new Uri("Resources/cylinder.png", UriKind.Relative));
                        figure = new Cylinder(5,1,10);
                        break;
                    case 3:
                        imageSource = new BitmapImage(new Uri("Resources/cone.png", UriKind.Relative));
                        figure = new Cone(4,2,8);
                        break;
                }
            }

            var button = new Button();
            button.Click += SelectFigure;
            var image = new Image();
            image.Source = imageSource;
            button.Content = image;

            WrapPanel.Children.Add(button);
            _buttonFigures.Add(button, figure);
            SelectFigure(button, null);
        }

        private void SelectFigure(object sender, RoutedEventArgs e)
        {
            CubePanel.Visibility = Visibility.Collapsed;
            SpherePanel.Visibility = Visibility.Collapsed;
            CylinderPanel.Visibility = Visibility.Collapsed;
            
            _selectedFigureButton = (Button) sender;
            var figure = _buttonFigures[_selectedFigureButton];

            if (figure is Cube cube)
            {
                CubePanel.Visibility = Visibility.Visible;
                CubePanelX.Value = cube.XLength;
                CubePanelY.Value = cube.YLength;
                CubePanelZ.Value = cube.ZLength;
            }
            else if (figure is Sphere sphere)
            {
                SpherePanel.Visibility = Visibility.Visible;
                SpherePanelRadius.Value = sphere.Radius;
                SpherePanelPsi.Value = sphere.Psi;
                SpherePanelPhi.Value = sphere.Phi;
            }
            else if (figure is Cylinder cylinder)
            {
                CylinderPanel.Visibility = Visibility.Visible;
                CylinderPanelHeight.Value = cylinder.Height;
                CylinderPanelRadius.Value = cylinder.Radius;
                CylinderPanelPhi.Value = cylinder.Phi;
            }
            else if (figure is Cone cone)
            {
                CylinderPanel.Visibility = Visibility.Visible;
                CylinderPanelHeight.Value = cone.Height;
                CylinderPanelRadius.Value = cone.Radius;
                CylinderPanelPhi.Value = cone.Phi;
            }
            
            foreach (var child in WrapPanel.Children)
            {
                var button = (Button) child;
                button.Opacity = 0.5;
            }

            BasicTranslationX.Value = figure.Translation.X;
            BasicTranslationY.Value = figure.Translation.Y;
            BasicTranslationZ.Value = figure.Translation.Z;
            BasicScaleX.Value = figure.Scale.X;
            BasicScaleY.Value = figure.Scale.Y;
            BasicScaleZ.Value = figure.Scale.Z;
            BasicRotationX.Value = figure.Rotation.X;
            BasicRotationY.Value = figure.Rotation.Y;
            BasicRotationZ.Value = figure.Rotation.Z;
            
            _selectedFigureButton.Opacity = 1;
        }

        private void BasicParameterChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_selectedFigureButton == null)
                return;
            
            int tag = int.Parse((string) ((DoubleUpDown)sender).Tag);
            var figure = _buttonFigures[_selectedFigureButton];
            if (e.NewValue is null) return;
            
            float value = (float)(double) e.NewValue;
            switch (tag)
            {
                case 0:
                    figure.Translation = new Vector3(value, figure.Translation.Y, figure.Translation.Z);
                    break;
                case 1:
                    figure.Translation = new Vector3( figure.Translation.X, value, figure.Translation.Z);
                    break;
                case 2:
                    figure.Translation = new Vector3( figure.Translation.X, figure.Translation.Y, value);
                    break;
                case 3:
                    figure.Scale = new Vector3(value, figure.Scale.Y, figure.Scale.Z);
                    break;
                case 4:
                    figure.Scale = new Vector3( figure.Scale.X, value, figure.Scale.Z);
                    break;
                case 5:
                    figure.Scale = new Vector3( figure.Scale.X, figure.Scale.Y, value);
                    break;
                case 6:
                    figure.Rotation = new Vector3(value, figure.Rotation.Y, figure.Rotation.Z);
                    break;
                case 7:
                    figure.Rotation = new Vector3( figure.Rotation.X, value, figure.Rotation.Z);
                    break;
                case 8:
                    figure.Rotation = new Vector3( figure.Rotation.X, figure.Rotation.Y, value);
                    break;
            }
            UpdateScene();
        }

        private void DoubleParameterChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
             if (_selectedFigureButton == null)
                 return;
             
             int tag = int.Parse((string) ((DoubleUpDown)sender).Tag);
             var figure = _buttonFigures[_selectedFigureButton];
             if (e.NewValue is null) return;
             
             float value = (float)(double) e.NewValue;
             switch (tag)
             {
                 case 10:
                     var cube0 = (Cube) figure;
                     cube0.XLength = value;
                     break;
                 case 11:
                     var cube1 = (Cube) figure;
                     cube1.YLength = value;
                     break;
                 case 12:
                     var cube2 = (Cube) figure;
                     cube2.ZLength = value;
                     break;
                 case 20:
                     var sphere = (Sphere) figure;
                     sphere.Radius = value;
                     break;
                 case 30:
                     if (figure is Cone cone1)
                     {
                         cone1.Height = value;
                     }
                     else
                     {
                         var cylinder1 = (Cylinder) figure;
                         cylinder1.Height = value;
                     }
                     break;
                 case 31:
                     if (figure is Cone cone2)
                     {
                         cone2.Radius = value;
                     }
                     else
                     {
                         var cylinder2 = (Cylinder) figure;
                         cylinder2.Radius = value;
                     }

                     break;
             }           
             UpdateScene();
        }

        private void IntParameterChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_selectedFigureButton == null)
              return;

            int tag = int.Parse((string) ((IntegerUpDown)sender).Tag);
            var figure = _buttonFigures[_selectedFigureButton];
            if (e.NewValue is null) return;

            int value = (int) e.NewValue;
            switch (tag)
            {
              case 21:
                  var sphere1 = (Sphere) figure;
                  sphere1.Psi = value;
                  break;
              case 22:
                  var sphere2 = (Sphere) figure;
                  sphere2.Phi = value;
                  break;
             case 32:
                 if (figure is Cone cone1)
                 {
                     cone1.Phi = value;
                 }
                 else
                 {
                     var cylinder1 = (Cylinder) figure;
                     cylinder1.Phi = value;
                 }
                 break;
            }                      
            UpdateScene();
        }
    }
}