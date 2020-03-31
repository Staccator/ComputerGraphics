using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ShapeDrafter.MathOperations;
using ShapeDrafter.Models;
using Xceed.Wpf.Toolkit;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private readonly Dictionary<Button, Light> _buttonLights = new Dictionary<Button, Light>();
        private Button _selectedLightButton;
        private int _lightCount = 0;

        private void CreateLightClick(object sender, RoutedEventArgs e)
        {
            var button = new Button();
            button.Content = (++_lightCount).ToString();
            button.Click += SelectLight;
            Light light = sender is Light lighte ? lighte :  new Light(new Vector3(1,1,1), 1, 0.01f, 0.002f, new Vector3(15,15,17));

            WrapPanelLight.Children.Add(button);
            _buttonLights.Add(button, light);
            SelectLight(button, null);
            UpdateLightReferences();
        }

        private void DeleteLightClick(object sender, RoutedEventArgs e)
        {
            if (WrapPanelLight.Children.Count <= 1)
                return;
                
            WrapPanelLight.Children.Remove(_selectedLightButton);
            _buttonLights.Remove(_selectedLightButton);
            var first = WrapPanelLight.Children[0];
            SelectLight(first,null);
            UpdateLightReferences();
        }
        
        private void SelectLight(object sender, RoutedEventArgs e)
        {
            _selectedLightButton = (Button) sender;
            var light = _buttonLights[_selectedLightButton];

            foreach (var child in WrapPanelLight.Children)
            {
                var button = (Button) child;
                button.Opacity = 0.5;
            }

            LightPanelAc.Value = Math.Round(light.Ac,3);
            LightPanelAl.Value = light.Al;
            LightPanelAq.Value = light.Aq;
            LightPanelX.Value = light.Position.X;
            LightPanelY.Value = light.Position.Y;
            LightPanelZ.Value = light.Position.Z;
            _selectedLightButton.Opacity = 1;
        }


        private void UpdateLightReferences()
        {
            var lights = _buttonLights.Select(pair => pair.Value).ToList();
            Lightning.Lights = lights;
            UpdateScene();
        }

        private void LightParameterChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int tag = int.Parse((string) ((DoubleUpDown)sender).Tag);
            var light = _buttonLights[_selectedLightButton];
            if (e.NewValue is null) return;
            
            float value = (float)(double) e.NewValue;
            
            switch (tag)
            {
                case 0:
                    light.Ac = value;
                    break;
                case 1:
                    light.Al = value;
                    break;
                case 2:
                    light.Aq = value;
                    break;
                case 3:
                    light.Position = new Vector3(value, light.Position.Y, light.Position.Z);
                    break;
                case 4:
                    light.Position = new Vector3(light.Position.X,value,  light.Position.Z);
                    break;
                case 5:
                    light.Position = new Vector3(light.Position.X, light.Position.Y, value);
                    break;
            }
            UpdateLightReferences();
        }
    }
}