using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using Microsoft.Win32;
using ShapeDrafter.Figures;
using ShapeDrafter.Graphics;
using ShapeDrafter.MathOperations;
using ShapeDrafter.Models;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private void SerializeToFile(string fileName)
        {
            var figures = _buttonFigures.Select(pair => pair.Value).ToArray();
            var cameras = _buttonCameras.Select(pair => pair.Value).ToArray();
            var lights = _buttonLights.Select(pair => pair.Value).ToArray();
            var scene = new Scene(figures, cameras, lights);
            XmlSerializer ser = new XmlSerializer(typeof(Scene));

            TextWriter writer = new StreamWriter(fileName);
            ser.Serialize(writer, scene);
            writer.Close();
        }

        private void LoadSceneFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog  
            {  
                Title = "Load scene from a file",  
                CheckFileExists = true,  
                CheckPathExists = true,  
                DefaultExt = "xml",  
                Filter = "xml files (*.xml)|*.xml",  
                FilterIndex = 2,  
                RestoreDirectory = true,  
            };

            var result = ofd.ShowDialog();
            
            if (result.HasValue && result.Value)
            {
                string fileName = ofd.FileName;
                var mySerializer = new XmlSerializer(typeof(Scene));
                var myFileStream = new FileStream(fileName , FileMode.Open);
                var scene = (Scene) mySerializer.Deserialize(myFileStream);
                
                ClearScene();
                LoadScene(scene);
            }
        }

        private void SaveSceneFileClick(object sender, RoutedEventArgs e)
            {
                var sfd = new SaveFileDialog
                {
                    RestoreDirectory = true,
                    Title = "Save scene to a file",
                    Filter = "xml files (*.xml)|*.xml",
                    DefaultExt = "xml"
                };


                var result = sfd.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    string fileName = sfd.FileName;
                    SerializeToFile(fileName);
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
                Lightning.Texture = new Texture(bitmap);
                Lightning.Texturing = true;
            }
            
            UpdateScene();
        }
        
        private void LoadBumpMapClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Load Bumpmap file";
            ofd.Filter = "jpg|*.jpg|png|*.png";
            if (ofd.ShowDialog().HasValue)
            {
                if (ofd.FileName == "") return;
                var bitmap = new Bitmap(ofd.FileName);
                var bumpMap = new Texture(bitmap);
                bumpMap.ConvertToBumpMap();
                Lightning.BumpMap = bumpMap;
                Lightning.BumpMapping = true;
            }
            
            UpdateScene();
        }

        private void ClearScene()
        {
            WrapPanel.Children.Clear();
            _buttonFigures.Clear();
            
            WrapPanelCamera.Children.Clear();
            _buttonCameras.Clear();
            
            WrapPanelLight.Children.Clear();
            _buttonLights.Clear();
        }

        private void LoadScene(Scene scene)
        {
            foreach (var camera in scene.Cameras)
            {
                CreateCameraClick(camera, null);
            }
            
            foreach (var light in scene.Lights)
            {
                CreateLightClick(light, null);
            }
            
            foreach (var figure in scene.Figures)
            {
                CreateShapeClick(figure,null);
            }
        }
    }
}