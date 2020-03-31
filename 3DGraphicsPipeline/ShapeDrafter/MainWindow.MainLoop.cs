using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ShapeDrafter.MathOperations;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    public class FrameLoad
    {
        public bool _loading;
    }
    
    public partial class MainWindow
    {
        Stopwatch sw = new Stopwatch();
        private readonly FrameLoad _frameLoad= new FrameLoad();

        private void StartMainLoop()
        {
            CompositionTarget.Rendering += Update;
        }

        private void Update(object sender, EventArgs e)
        {
            sw.Restart();

            UpdateScene();
            
            sw.Stop();
            Console.WriteLine("Frame Rate : " + 1 / sw.Elapsed.TotalSeconds);
        }

        private void ShowFrameRate()
        {
            sw.Stop();
            FpsBlock.Text = (1 / sw.Elapsed.TotalSeconds).ToString("N1");
            // Console.WriteLine("Frame Rate : " + 1 / sw.Elapsed.TotalSeconds);
        }

        private void UpdateScene()
        {
            Console.WriteLine("\nFRAME CREATION START");
            sw.Restart();
            bool backFaceCulling = GlobalOptions.BackFaceCulling;
            
            //Getting all triangles in world coordinates
            var worldTriangles = new List<Triangle>();
            foreach (var buttonFigure in _buttonFigures)
            {
                var figure = buttonFigure.Value;
                worldTriangles.AddRange(figure.WorldTriangles);
            }
            
            //Camera stuff
            var camera = _buttonCameras[_selectedCameraButton];
            camera.RefreshProjectionMatrix();
            var cameraMatrix = camera.ProjectionMatrix * camera.ViewMatrix;
            
            //Calculate Screen Coordinates
            for (var i = 0; i < worldTriangles.Count; i++)
            {
                var triangle = worldTriangles[i];
                triangle.number = i;
                triangle.CalculateScreenCoordinates(cameraMatrix);
            }

            Console.WriteLine("BEFORE CULLING = " + worldTriangles.Count);
            // foreach (var triangle in worldTriangles) Console.WriteLine(triangle);
            
            if (backFaceCulling)
                worldTriangles = worldTriangles.Where(t => !t.BackFaced).ToList();

            Console.WriteLine("AFTER CULLING = " + worldTriangles.Count);
            // foreach (var triangle in worldTriangles) Console.WriteLine(triangle);
            
            var clippedTriangles = new List<Triangle>();
            foreach (var triangle in worldTriangles)
            {
                clippedTriangles.AddRange(Clipping.ClipsTriangle(triangle));
            }
            clippedTriangles = clippedTriangles.Where(t => t.IsGoodTriangle()).ToList();

            Console.WriteLine("AFTER CLIPPING = " + clippedTriangles.Count);
            // foreach (var triangle in clippedTriangles) Console.WriteLine(triangle);
            
            List<Texel>[] texelsArray = new List<Texel>[clippedTriangles.Count];
            
            Parallel.For(0, texelsArray.Length, i =>
            {
                var tri = clippedTriangles[i];
                var vertices = Renderer.Rasterize(tri);
                texelsArray[i] = vertices.Select(v => Lightning.CalculateTexel(v)).ToList();
            });

            //Draw pixels to buffer
            var buffer = CreateNewBuffer();
            var depthBuffer = CreateNewDepthBuffer();
            ClearBuffer(buffer);
            Parallel.For(0, texelsArray.Length, body: i => { DrawTexels(texelsArray[i], buffer, depthBuffer);});
            CommitDrawWithBuffer(buffer);

            ShowFrameRate();
            Console.WriteLine("Camera Position = " + camera.P);
            // Console.WriteLine("Front vector = " + camera.F);
        }
    }
}