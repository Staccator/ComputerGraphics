using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media;
using static ShapeDrafter.Drawing.DrawingExtensions;
using Color = System.Drawing.Color;

namespace ShapeDrafter.Models
{
    public class Polygon
    {
        private static readonly Color DefaultColor = Color.White;
        private readonly Color _color;
        private readonly MainWindow _window;

        public Polygon(Color color)
        {
            _color = color;
            _window = MainWindow.instance;
        }

        public List<ParallelEdges> ParallelPairs { get; } = new List<ParallelEdges>();

        public List<Edge> Edges { get; set; } = new List<Edge>();
        public List<Vertex> Vertices { get; set; } = new List<Vertex>();

        public void SetupParallelEdges(int edge1Id, int edge2Id, bool parallel)
        {
            var edge1 = Edges.First(e => e.Id == edge1Id);
            var edge2 = Edges.First(e => e.Id == edge2Id);

            var vertex11 = Vertices.First(v => v.Id == edge1.Vertices[0].Id);
            var vertex12 = Vertices.First(v => v.Id == edge1.Vertices[1].Id);
            var vertex21 = Vertices.First(v => v.Id == edge2.Vertices[0].Id);
            var vertex22 = Vertices.First(v => v.Id == edge2.Vertices[1].Id);

            var intEdges = new List<int> {edge1Id, edge2Id};
            var intVertices = new List<int>();

            if (vertex11.Point.X < vertex12.Point.X)
            {
                intVertices.Add(vertex11.Id);
                intVertices.Add(vertex12.Id);
            }
            else
            {
                intVertices.Add(vertex12.Id);
                intVertices.Add(vertex11.Id);
            }

            if (vertex21.Point.X < vertex22.Point.X)
            {
                intVertices.Add(vertex21.Id);
                intVertices.Add(vertex22.Id);
            }
            else
            {
                intVertices.Add(vertex22.Id);
                intVertices.Add(vertex21.Id);
            }

            var parallelEdges = new ParallelEdges
            {
                Parallel = parallel,
                Edges = intEdges,
                Vertices = intVertices
            };
            
            ParallelPairs.Add(parallelEdges);
        }

        List<Point> DotsToClear = new List<Point>();

        private Color[] _dotColors = new Color[]
        {
            Color.Green, Color.Blue, Color.Black, Color.Purple, Color.Yellow, Color.Coral
        };
        public void Redraw(bool clear = false)
        {
            if (clear)
            {
                foreach (var vertex in Vertices) _window.DrawDot(vertex.Point, Color.White);
            }
            else
            {
                foreach (var vertex in Vertices) _window.DrawDot(vertex.Point, Color.Crimson);
            }

            var parallels = new List<int>();
            var perpendiculars = new List<int>();

            foreach (var parallelPair in ParallelPairs)
                if (parallelPair.Parallel)
                {
                    parallels.Add(parallelPair.Edges[0]);
                    parallels.Add(parallelPair.Edges[1]);
                }
                else
                {
                    perpendiculars.Add(parallelPair.Edges[0]);
                    perpendiculars.Add(parallelPair.Edges[1]);
                }

            foreach (var dot in DotsToClear)
            {
                _window.DrawDot(dot, Color.White);
            }
            DotsToClear = new List<Point>();
            
            int i = 0;

            if (!clear)
            {
                foreach (var parallelPair in ParallelPairs)
                {
                    foreach (var edgeInt in parallelPair.Edges)
                    {
                        var edge = Edges.First(e => e.Id == edgeInt);
                        var start = edge.Points.First();
                        var end = edge.Points.Last();
                        var midPoint = new Point((start.X  + end.X)/2, (start.Y  + end.Y)/2);
                        _window.DrawDot(midPoint, _dotColors[i]);
                        DotsToClear.Add(midPoint);
                    }

                    i = (i + 1) % _dotColors.Length;
                }
            }
            
            foreach (var edge in Edges)
            {
                if (clear)
                    _window.DrawPoints(edge.Points, Color.White);
                else
                {
                    _window.DrawPoints(edge.Points, _color);
                }
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

        public void DeleteVertex(Vertex vertex)
        {
            if (Vertices.Count <= 3)
                return;

            var prevVertex = vertex.Edges[0].Vertices[0];
            var nextVertex = vertex.Edges[1].Vertices[1];
            var prevEdge = vertex.Edges[0];
            var nextEdge = vertex.Edges[1];

            _window.DrawPoints(vertex.Edges[0].Points, DefaultColor);
            _window.DrawPoints(vertex.Edges[1].Points, DefaultColor);
            _window.DrawDot(vertex.Point, DefaultColor);

            var newEdgePoints = GetLine(nextVertex.Point.X, nextVertex.Point.Y, prevVertex.Point.X, prevVertex.Point.Y);
            var maxEdgeId = Edges.Select(e => e.Id).Max();
            var newEdge = new Edge(maxEdgeId + 1, newEdgePoints);
            prevVertex.Edges[1] = nextVertex.Edges[0] = newEdge;
            newEdge.Vertices[0] = prevVertex;
            newEdge.Vertices[1] = nextVertex;

            Edges.Add(newEdge);
            Vertices.Remove(vertex);
            Edges.Remove(prevEdge);
            Edges.Remove(nextEdge);

            ParallelPairs.RemoveAll(pair => pair.Vertices.Contains(vertex.Id));

            _window.RedrawAll();
        }

        public void GoodShiftVertex(Vertex shiftedVertex, Point point)
        {
            ShiftVertex(shiftedVertex, point);
            Parallelize(shiftedVertex);
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

        public void AddVertexOnEdge(Point edgePoint, Edge edge)
        {
            _window.DrawPoints(edge.Points, DefaultColor);
            
            ParallelPairs.RemoveAll(pair => pair.Edges.Contains(edge.Id));

            var prevVertex = edge.Vertices[0];
            var nextVertex = edge.Vertices[1];

            var maxVertexId = Vertices.Select(v => v.Id).Max();
            var maxEdgeId = Edges.Select(e => e.Id).Max();
            var prevEdgePoints = GetLine(edgePoint.X, edgePoint.Y, prevVertex.Point.X, prevVertex.Point.Y);
            var nextEdgePoints = GetLine(edgePoint.X, edgePoint.Y, nextVertex.Point.X, nextVertex.Point.Y);

            var newVertex = new Vertex(maxVertexId + 1, edgePoint);
            var newPrevEdge = new Edge(maxEdgeId + 1, prevEdgePoints);
            var newNextEdge = new Edge(maxEdgeId + 2, nextEdgePoints);

            newVertex.Edges[0] = newPrevEdge;
            newVertex.Edges[1] = newNextEdge;

            newPrevEdge.Vertices[0] = prevVertex;
            newPrevEdge.Vertices[1] = newVertex;
            newNextEdge.Vertices[0] = newVertex;
            newNextEdge.Vertices[1] = nextVertex;

            prevVertex.Edges[1] = newPrevEdge;
            nextVertex.Edges[0] = newNextEdge;

            Edges.Remove(edge);
            Edges.Add(newPrevEdge);
            Edges.Add(newNextEdge);
            Vertices.Add(newVertex);

            _window.RedrawAll();
        }

        public void ShiftEdge(Edge shiftedEdge, Point diff)
        {
            var prevVertex = shiftedEdge.Vertices[0];
            var nextVertex = shiftedEdge.Vertices[1];
            var prevEdge = prevVertex.Edges[0];
            var nextEdge = nextVertex.Edges[1];

            _window.DrawPoints(prevEdge.Points, DefaultColor);
            _window.DrawPoints(shiftedEdge.Points, DefaultColor);
            _window.DrawPoints(nextEdge.Points, DefaultColor);

            _window.DrawDot(prevVertex.Point, DefaultColor);
            _window.DrawDot(nextVertex.Point, DefaultColor);

            prevVertex.Point = prevVertex.Point.Add(diff);
            nextVertex.Point = nextVertex.Point.Add(diff);

            var prevEdgePoints = GetLine(prevVertex.Point.X, prevVertex.Point.Y, prevEdge.Vertices[0].Point.X,
                prevEdge.Vertices[0].Point.Y);
            var shiftedEdgePoints =
                GetLine(prevVertex.Point.X, prevVertex.Point.Y, nextVertex.Point.X, nextVertex.Point.Y);
            var nextEdgePoints = GetLine(nextVertex.Point.X, nextVertex.Point.Y, nextEdge.Vertices[1].Point.X,
                nextEdge.Vertices[1].Point.Y);
            prevEdge.Points = prevEdgePoints;
            shiftedEdge.Points = shiftedEdgePoints;
            nextEdge.Points = nextEdgePoints;
            
            Redraw();
        }

        public void ClearPolygon()
        {
            foreach (var edge in Edges) _window.DrawPoints(edge.Points, DefaultColor);

            foreach (var vertex in Vertices) _window.DrawDot(vertex.Point, DefaultColor);
        }

        public void ShiftAll(Edge shiftedEdge, Point diff)
        {
            ClearPolygon();
            foreach (var edge in Edges) edge.Points = edge.Points.Select(p => p.Add(diff)).ToList();

            foreach (var vertex in Vertices) vertex.Point = vertex.Point.Add(diff);

            _window.RedrawAll();
        }

        public void Parallelize(Vertex changedVertex = null)
        {
            foreach (var parallelEdges in ParallelPairs.OrderBy(p => p.Parallel))
            {
                var parallel = parallelEdges.Parallel;
                var parallelEdgesVertices = parallelEdges.Vertices;
                
                if (parallel && changedVertex != null && (changedVertex.Id == parallelEdgesVertices[2] ||
                                              changedVertex.Id == parallelEdgesVertices[3]))
                {
                    var first2 = parallelEdgesVertices.Take(2);
                    var last2 = parallelEdgesVertices.Skip(2).ToList();
                    last2.AddRange(first2);
                    parallelEdgesVertices = last2;
                }

                var point11 = Vertices.First(v => v.Id == parallelEdgesVertices[0]).Point;
                var point12 = Vertices.First(v => v.Id == parallelEdgesVertices[1]).Point;
                var point21 = Vertices.First(v => v.Id == parallelEdgesVertices[2]).Point;
                var point22 = Vertices.First(v => v.Id == parallelEdgesVertices[3]).Point;
                var vert22 = Vertices.First(v => v.Id == parallelEdgesVertices[3]);

                var length1 = point11.Length(point12);
                var length2 = point21.Length(point22) + 0.6;

                var diffP1 = point12.Minus(point11);
                if (!parallel)
                {
                   diffP1 = new Point(diffP1.Y, -diffP1.X); 
                }
                
                var newP22 = point21.Add(diffP1.Multiply(length2 / length1));
                ShiftVertex(vert22, newP22);
            }
        }

        public void Swap<T>(ref T p1, ref T p2)
        {
            var temp = p1;
            p1 = p2;
            p2 = temp;
        }

        public void DeleteEdgeRelation(Edge edge)
        {
            ParallelPairs.RemoveAll(pair => pair.Edges.Contains(edge.Id));
            _window.RedrawAll();
        }
    }
}