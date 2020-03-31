using System.Drawing;
using ShapeDrafter.Drawing;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private void Delete(Point point)
        {
            foreach (var polygon in _polygons)
            {
                var vertex = polygon.DetectVertex(point);
                if (vertex != null)
                {
                    polygon.DeleteVertex(vertex);
                    return;
                }
            }
            
            foreach (var polygon in _polygons)
            {
                var (edgePoint, edge) = polygon.DetectEdge(point);
                if (edge != null)
                {
                    polygon.DeleteEdgeRelation(edge);
                    return;
                }
            }
        }

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
            
            foreach (var circle in _circles)
            {
                for (int i = 0; i < circle.Points.Count; i++)
                {
                    var circlePoint = circle.Points[i];
                    if (circlePoint .Length(point) < 10)
                    {
                        circle.Redraw(true);
                        _circles.Remove(circle);
                        return;
                    }
                }
            }
            
        }
    }
}