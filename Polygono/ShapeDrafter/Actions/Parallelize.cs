using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows;
using ShapeDrafter.Models;
using Point = System.Drawing.Point;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private bool _firstParallel = true;
        private Polygon _lastPolygon;
        private int _lastEdgeId;
        private bool _parallel = true;

        private void Parallelize(Point point, bool parallel = true)
        {
            foreach (var polygon in _polygons)
            {
                var (edgePoint, edge) = polygon.DetectEdge(point);
                if (edge != null)
                {
                    _parallel = parallel;
                    SetupParallel(polygon, edge, edgePoint);
                    return;
                }
            }
        }

        private void SetupParallel(Polygon polygon, Edge edge, Point edgePoint)
        {
            if (polygon.ParallelPairs.Any(pair => pair.Edges.Contains(edge.Id)))
            {
                Console.WriteLine("You can't add, relation is already here");
                return;
            }
            
            if (_firstParallel)
            {
                _lastPolygon = polygon;
                _lastEdgeId = edge.Id;
                DrawPoints(edge.Points,_parallel? Color.Black : Color.Purple);
                _firstParallel = false;
                return;
            }

            if (_parallel)
            {
                var lastEdge = polygon.Edges.First(e => e.Id == _lastEdgeId);
                if (edge.Id == lastEdge.Vertices[0].Edges[0].Id || edge.Id == lastEdge.Vertices[1].Edges[1].Id)
                    return;
            }

            if (polygon == _lastPolygon && edge.Id != _lastEdgeId)
            {
                polygon.SetupParallelEdges(_lastEdgeId, edge.Id, _parallel);
                _firstParallel = true;
                polygon.Parallelize();
            }

            RedrawAll();
        }
    }
}