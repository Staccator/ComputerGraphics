using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media;
using ShapeDrafter.Graphics;
using static ShapeDrafter.Drawing.DrawingExtensions;
using Color = System.Drawing.Color;

namespace ShapeDrafter.Models
{
    public class Polygon
    {
        private static readonly Color DefaultColor = Color.White;
        private readonly Color _color;
        protected readonly MainWindow _window = MainWindow.Instance;
        public List<Edge> Edges { get; set; } = new List<Edge>();
        public List<Vertex> Vertices { get; set; } = new List<Vertex>();

        public Polygon(List<Point> points, Color color)
        {
            _color = color;
            Vertices = points.Select(p => new Vertex(0, p)).ToList();
            for (int i = 0; i < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[i==points.Count-1 ? 0 : i+1];
                var edge = new Edge(0, GetLine(p1.X, p1.Y, p2.X, p2.Y));
                Edges.Add(edge);
            }
        }
        
        public Polygon(Color color)
        {
            _color = color;
            _window = MainWindow.Instance;
        }

        public void Redraw(bool clear = false)
        {
            foreach (var edge in Edges)
            {
                _window.DrawPoints(edge.Points, clear ? DefaultColor : _color);
            }
        }

        public Vertex DetectVertex(Point point)
        {
            foreach (var vertex in Vertices)
                if (vertex.Point.Length(point) < 10)
                    return vertex;
            return null;
        }

        public (Point, Edge) DetectEdge(Point point)
        {
            foreach (var edge in Edges)
            {
                var edgePoints = edge.Points;
                for (var i = 0; i < edgePoints.Count; i++)
                {
                    var edgePoint = edgePoints[i];
                    if (edgePoint.Length(point) < 10) return (edgePoint, edge);
                }
            }

            return (Point.Empty, null);
        }

        public void GoodShiftVertex(Vertex shiftedVertex, Point point)
        {
            ShiftVertex(shiftedVertex, point);
        }

        public void ShiftVertex(Vertex shiftedVertex, Point point)
        {
            var prevVertex = shiftedVertex.Edges[0].Vertices[0];
            var nextVertex = shiftedVertex.Edges[1].Vertices[1];
            var prevEdge = shiftedVertex.Edges[0];
            var nextEdge = shiftedVertex.Edges[1];

            _window.DrawPoints(shiftedVertex.Edges[0].Points, DefaultColor);
            _window.DrawPoints(shiftedVertex.Edges[1].Points, DefaultColor);
            _window.DrawDot(shiftedVertex.Point, DefaultColor);

            var prevEdgePoints = GetLine(point.X, point.Y, prevVertex.Point.X, prevVertex.Point.Y);
            var nextEdgePoints = GetLine(point.X, point.Y, nextVertex.Point.X, nextVertex.Point.Y);
            prevEdge.Points = prevEdgePoints;
            nextEdge.Points = nextEdgePoints;

            shiftedVertex.Point = point;
        }

        public void ShiftAll(Point diff)
        {
            Redraw(true);
            foreach (var edge in Edges) edge.Points = edge.Points.Select(p => p.Add(diff)).ToList();

            foreach (var vertex in Vertices) vertex.Point = vertex.Point.Add(diff);
        }
    }
}