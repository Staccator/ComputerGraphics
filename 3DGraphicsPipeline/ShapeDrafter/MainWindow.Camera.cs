using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ShapeDrafter.Models;
using Xceed.Wpf.Toolkit;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private readonly Dictionary<Button, Camera> _buttonCameras = new Dictionary<Button, Camera>();
        private Button _selectedCameraButton;
        private int _cameraCount = 0;

        private void CreateCameraClick(object sender, RoutedEventArgs e)
        {
            var button = new Button();
            button.Content = (++_cameraCount).ToString();
            button.Click += SelectCamera;
            Camera camera = sender is Camera cameras ? cameras : new Camera(2f, 20, 0.7f);

            WrapPanelCamera.Children.Add(button);
            _buttonCameras.Add(button, camera);
            SelectCamera(button, null);
        }

        private void SelectCamera(object sender, RoutedEventArgs e)
        {
            _selectedCameraButton = (Button) sender;
            var camera = _buttonCameras[_selectedCameraButton];

            foreach (var child in WrapPanelCamera.Children)
            {
                var button = (Button) child;
                button.Opacity = 0.5;
            }

            CameraPanelFov.Value = Math.Round(camera.Fov * (180 / Math.PI),3);
            CameraPanelNear.Value = camera.Near;
            CameraPanelFar.Value = camera.Far;
            _selectedCameraButton.Opacity = 1;
            
            UpdateScene();
        }

        private void DeleteCameraClick(object sender, RoutedEventArgs e)
        {
            if (WrapPanelCamera.Children.Count <= 1)
                return;
                
            WrapPanelCamera.Children.Remove(_selectedCameraButton);
            _buttonCameras.Remove(_selectedCameraButton);
            var first = WrapPanelCamera.Children[0];
            SelectCamera(first,null);
        }

        private void CameraParameterChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int tag = int.Parse((string) ((DoubleUpDown)sender).Tag);
            var camera = _buttonCameras[_selectedCameraButton];
            if (e.NewValue is null) return;
            
            float value = (float)(double) e.NewValue;
            
            switch (tag)
            {
                case 0:
                    camera.Fov = value * (float)(Math.PI / 180);
                    break;
                case 1:
                    if (camera.Far < value)
                    {
                        CameraPanelNear.Value = camera.Far;
                        camera.Near = camera.Far;
                        return;
                    }
                    camera.Near = value;
                    break;
                case 2:
                    if (camera.Near > value)
                    {
                        CameraPanelFar.Value = camera.Near;
                        camera.Far = camera.Near;
                        return;
                    }
                    camera.Far = value;
                    break;
            }
            
            UpdateScene();
        }
    }
}