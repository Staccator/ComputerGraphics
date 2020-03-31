using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ShapeDrafter.Graphics;
using ShapeDrafter.Models;
using Color = System.Drawing.Color;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private Vector2F _lastMousePosition;
        private DateTime _lastAccessMouse = DateTime.Now;
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if ((DateTime.Now - _lastAccessMouse).TotalSeconds < 0.15)
            {
                return;
            }
            _lastAccessMouse = DateTime.Now;
            
            var camera = _buttonCameras[_selectedCameraButton];
            var position = e.GetPosition(PaintSurface);
            var mousePosition = new Vector2F((float) position.X, (float) position.Y);

            var diff = mousePosition - _lastMousePosition;
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (diff.Direction())
                {
                    case 3:
                        camera.RotateHorizontally(-RotateDiff);
                        break;
                    case 1:
                        camera.RotateHorizontally(RotateDiff);
                        break;
                    case 0:
                        camera.RotateVertically(RotateDiff);
                        break;
                    case 2:
                        camera.RotateVertically(-RotateDiff);
                        break;
                }
                UpdateScene();
            }
                
            _lastMousePosition = mousePosition;
        }

        private void GlobalOptionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            int tag = int.Parse((string) ((CheckBox)sender).Tag);

            switch (tag)
            {
                case 0:
                    GlobalOptions.Perspective = !GlobalOptions.Perspective;
                    break;
                case 1:
                    GlobalOptions.Filling = !GlobalOptions.Filling;
                    break;
                case 2:
                    GlobalOptions.BackFaceCulling = !GlobalOptions.BackFaceCulling;
                    break;
                case 3:
                    GlobalOptions.ZBuffering = !GlobalOptions.ZBuffering;
                    break;
            }

            UpdateScene();
            Console.WriteLine($"{GlobalOptions.Perspective}  {GlobalOptions.Filling}  {GlobalOptions.BackFaceCulling}  {GlobalOptions.ZBuffering}");
        }

        private DateTime _lastAccessKey = DateTime.Now;
        private const float RotateDiff = (float)Math.PI / 55;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((DateTime.Now - _lastAccessKey).TotalSeconds < 0.2)
            {
                return;
            }
            _lastAccessKey = DateTime.Now;
            
            var camera = _buttonCameras[_selectedCameraButton];
            float diff = 1f;
            switch (e.Key)
            {
                case Key.OemSemicolon:
                    Console.WriteLine("semi");
                    UpdateScene();
                    break;
                case Key.A:
                    camera.MoveLeft();
                    UpdateScene();
                    break;
                case Key.D:
                    camera.MoveRight();
                    UpdateScene();
                    break;
                case Key.W:
                    camera.MoveUp();
                    UpdateScene();
                    break;
                case Key.S:
                    camera.MoveDown();
                    UpdateScene();
                    break;
                case Key.R:
                    camera.MoveFront();
                    UpdateScene();
                    break;
                case Key.F:
                    camera.MoveBack();
                    UpdateScene();
                    break;
                case Key.I:
                    camera.RotateHorizontally(-RotateDiff);
                    UpdateScene();
                    break;
                case Key.P:
                    camera.RotateHorizontally(RotateDiff);
                    UpdateScene();
                    break;
                case Key.U:
                    camera.RotateVertically(RotateDiff);
                    UpdateScene();
                    break;
                case Key.J:
                    camera.RotateVertically(-RotateDiff);
                    UpdateScene();
                    break;
            }
        }
        
        private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (e.NewValue != null)
            {
                var col = e.NewValue.Value;
                var lightColor = new Vector3(col.R, col.G, col.B) * (1 / 255f) * 1f;
                var light = _buttonLights[_selectedLightButton];
                light.Color = lightColor;
                
                UpdateScene();
            }
        }
    }
}