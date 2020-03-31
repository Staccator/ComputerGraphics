using System.Collections.Generic;
using System.Drawing;
using ShapeDrafter.Models;

namespace ShapeDrafter.Graphics
{
    public class ClippedPolygon
    {
        public List<Point> Points;
        public ColorPolygon ColorPoly;

        public ClippedPolygon(List<Point> points, ColorPolygon colorPoly)
        {
            Points = points;
            ColorPoly = colorPoly;
        }
    }
}