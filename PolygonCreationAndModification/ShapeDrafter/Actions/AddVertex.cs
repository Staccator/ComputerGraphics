using System;
using System.Drawing;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private void AddVertex(Point point)
        {
            foreach (var polygon in _polygons)
            {
                var (edgePoint, edge) = polygon.DetectEdge(point);
                if (edge != null)
                {
                    polygon.AddVertexOnEdge(edgePoint, edge);
                    return;
                }
            }
        }
    }
}