using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using ShapeDrafter.Drawing;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private bool _isShifting;
        private Vertex _shiftedVertex;
        private Edge _shiftedEdge;
        private Polygon _shiftedPolygon;
        private Circle _shiftedCircle;
        private Point _lastPoint;
        private WhatIsShifted _what;

        private void ShiftStart(Point point)
        {
            foreach (var polygon in _polygons)
            {
                var vertex = polygon.DetectVertex(point);
                if (vertex != null)
                {
                    bool breaks = false;
                    foreach (var pair in polygon.ParallelPairs)
                    {
                        var verts1 = polygon.Edges.First(e => e.Id == pair.Edges[0]).Vertices;
                        var verts2 = polygon.Edges.First(e => e.Id == pair.Edges[1]).Vertices;
                        if (verts1.Any(v => v.Id == vertex.Id) && verts2.Any(v => v.Id == vertex.Id))
                        {
                            breaks = true;
                            break;
                        } 
                    }

                    if (breaks)
                    {
                        break;
                    }
                    
                    _shiftedPolygon = polygon;
                    _shiftedVertex = vertex;
                    _isShifting = true;
                    _what = WhatIsShifted.Vertex;
                    return;
                }
            }
            
            foreach (var polygon in _polygons)
            {
                var (edgePoint, edge) = polygon.DetectEdge(point);
                if (edge != null)
                {
                    _lastPoint = point;
                    _shiftedPolygon = polygon;
                    _shiftedEdge = edge;
                    _isShifting = true;
                    _what = WhatIsShifted.Edge;
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
                        _lastPoint = circlePoint ;
                        _isShifting = true;
                        _what = WhatIsShifted.Radius;
                        _shiftedCircle = circle;
                        return;
                    }
                }
            }
        }
        private void ShapeShiftStart(Point intPoint)
        {
            foreach (var polygon in _polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    for (int i = 0; i < edge.Points.Count; i++)
                    {
                        var point = edge.Points[i];
                        if (point.Length(intPoint) < 10)
                        {
                            _lastPoint = point;
                            _isShifting = true;
                            _what = WhatIsShifted.Polygon;
                            _shiftedPolygon = polygon;
                            return;
                        }
                    }
                }
            }

            foreach (var circle in _circles)
            {
                for (int i = 0; i < circle.Points.Count; i++)
                {
                    var point = circle.Points[i];
                    if (point.Length(intPoint) < 10)
                    {
                        _lastPoint = point;
                        _isShifting = true;
                        _what = WhatIsShifted.Circle;
                        _shiftedCircle = circle;
                        return;
                    }
                }
            }
        }

        private void ShiftMove(Point point)
        {
            if (_isShifting)
            {
                if (point.X < 0 || point.X> _width || point.Y < 0 || point.Y > _height - 10)
                    return;
                var diff = new Point(point.X - _lastPoint.X, point.Y - _lastPoint.Y);
                switch (_what)
                {
                    case WhatIsShifted.Edge:
                        _shiftedPolygon.ShiftEdge(_shiftedEdge, diff);
                        _shiftedPolygon.Parallelize();
                        RedrawAll();
                        break;
                    case WhatIsShifted.Vertex:
                        _shiftedPolygon.GoodShiftVertex(_shiftedVertex,point);
                        RedrawAll();
                        break;
                    case WhatIsShifted.Polygon:
                        _shiftedPolygon.ShiftAll(_shiftedEdge, diff);
                        break;
                    case WhatIsShifted.Circle:
                        _shiftedCircle.ShiftAll(diff);
                        break;
                    case WhatIsShifted.Radius:
                        _shiftedCircle.ShiftRadius(point);
                        break;
                }
            }

            _lastPoint = point;
        }

        private void ShiftEnd()
        {
            _isShifting = false;
        }
    }
}