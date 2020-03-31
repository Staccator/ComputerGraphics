using System;
using System.Collections.Generic;
using System.Drawing;
using ShapeDrafter.Graphics;
using ShapeDrafter.MathOperations;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        List<float[]> Vectors = new List<float[]>()
        {
            new float[]{-1, 0, -1, 1},
            new float[]{-1, 0, 1, 1},
            new float[]{1, 0, 1, 1},
            new float[]{1, 0, -1, 1},
        };
        
        List<Point> pointsToClear = new List<Point>();
        public void GeneratePolygons(float alpha)
        {
            DrawPoints(pointsToClear,Color.White);
            pointsToClear.Clear();
            
            Matrix M1 = Matrix.ProjectionMatrix() * Matrix.ViewMatrix(alpha);
            Matrix M2 = M1 * Matrix.TranslationMatrix(new Vector3(0, 2, 0));
            Matrix M3 = M1 * Matrix.TranslationMatrix(new Vector3(1, 1, 0)) * Matrix.RotateZMatrix((float)Math.PI / 2);
            
            var pointsToDraw = DrawTetragon(Vectors, M1);
            var filling1 = ScanLine.PolygonFilling(pointsToDraw,out Color[,] xd);
            DrawPoints(filling1,Color.Green);
            
            var pointsToDraw2 = DrawTetragon(Vectors, M3);
            var filling2 = ScanLine.PolygonFilling(pointsToDraw2,out Color[,] xd2);
            DrawPoints(filling2,Color.Yellow);
            pointsToClear.AddRange(filling1);
            pointsToClear.AddRange(filling2);
//            ClearScreen();
            CommitDraw();
        }
        
        

        public List<Point> DrawTetragon(List<float[]> vectors, Matrix matrix)
        {
            List<Point> result = new List<Point>();
            foreach (var vector in vectors)
            {
                var ci = matrix * vector;
                var ni = new float[]
                {
                    ci[0] / ci[3],
                    ci[1] / ci[3],
                    ci[2] / ci[3],
                    1
                };
                var x = (ni[0] * 0.5 + 0.5) * 800;
                var y = (-ni[1] * 0.5 + 0.5) * 600;
                result.Add(new Point((int) x, (int) y));
            }

            return result;
        }
    }
}