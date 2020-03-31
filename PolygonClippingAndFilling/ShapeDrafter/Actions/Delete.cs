using System.Drawing;
using ShapeDrafter.Drawing;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private void DeleteShape(Point point)
        {
            foreach (var polygon in _polygons)
            {
                var (edgePoint, edge) = polygon.DetectEdge(point);
                if (edge != null)
                {
                    polygon.Redraw(true);
                    _polygons.Remove(polygon);
                    return;
                }
            }
        }
    }
}