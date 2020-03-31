using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ShapeDrafter.Models
{
    public class ColorPolygon : Polygon
    {
        public Color[,] ColorTab { get; }
        public List<Point> Interior;
        public Point Shift;

        public ColorPolygon(List<Point> points, Color color, Color[,] colorTab, List<Point> interior) : base(points, color)
        {
            ColorTab = colorTab;
            Interior = interior;
        }

        public void ShiftInterior(int speed)
        {
            Interior = Interior.Select(p => new Point(p.X + speed, p.Y)).ToList();
        }
    }
}