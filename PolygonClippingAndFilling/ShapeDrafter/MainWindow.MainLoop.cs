using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ShapeDrafter.Graphics;
using ShapeDrafter.Models;
using Color = System.Drawing.Color;
using Timer = System.Timers.Timer;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private int _speed = 1;
        private List<ClippedPolygon> _clipped = new List<ClippedPolygon>();
        private int _clearCounter = 0;
        private const int WindowWidth = 800;

        private void StartMainLoop()
        {
            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            if (!_running)
            {
                return;
            }
            foreach (var colorPolygon in _generated)
            {
                colorPolygon.Clear();
                colorPolygon.Vertices = colorPolygon.Vertices.Select(v => new Vertex(0, new Point(v.Point.X + _speed, v.Point.Y))).ToList();
//                colorPolygon.ShiftInterior(_speed);
                colorPolygon.Shift.X += _speed;
            }

            if (_clearCounter++ % 60 == 0)
            {
                ClearGenerated();
            }

            ClearClippedOnScreen();
            _clipped.Clear();
            foreach (var subject in _polygons)
            {
                foreach (var clip in _generated)
                {
                    var temppoly = SutherlandHodgman.GetIntersectedPolygon(
                        subject.Vertices.Select(p => new System.Windows.Point(p.Point.X, p.Point.Y)).ToArray(),
                        clip.Vertices.Select(p => new System.Windows.Point(p.Point.X, p.Point.Y)).ToArray());

                    if (temppoly.Length != 0)
                    {
                        var poly = temppoly.Select(p => new Point((int)p.X,(int)p.Y)).ToList();
                        var filling = ScanLine.PolygonFilling(poly,out Color[,] xd);
                        var polygon = new ClippedPolygon(filling,clip);
                        _clipped.Add(polygon);
                    }
                }
            }
            
            RedrawAll();
        }

        private void ClearClippedOnScreen()
        {
            for (int i = 0; i < _clipped.Count; i++)
            {
                var clipped = _clipped[i];
                DrawPoints(clipped.Points,Color.White);
            }
        }

        private void ClearGenerated()
        {
            for (int i = 0; i < _generated.Count; i++)
            {
                var generated = _generated[i];
                if (generated.Vertices.Min(v => v.Point.X) > Texture.Width)
                {
                    _generated.Remove(generated);
                }
            }
        }

        private void OnSpeedChange(object sender, ScrollEventArgs e)
        {
            int speed = (int)(e.NewValue * 10);
            if (speed == 0)
            {
                speed++;
            }
            _speed = Math.Max(Math.Min(5,speed),1);
        }
    }
}