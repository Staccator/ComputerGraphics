using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Media;
using ShapeDrafter.Models;
using static ShapeDrafter.Drawing.DrawingExtensions;
using Color = System.Drawing.Color;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private bool _started;
        
        private Vertex _lastVertex;
        private Polygon _newPolygon;
        private List<Polygon> _polygons = new List<Polygon>();
        public void ClearUnfinishedPolygon()
        {
            if (_started)
            {
                _newPolygon.Redraw(true);
                _newPolygon = null;
                _started = false;
            }
        }
        
        private void DrawPolygon(Point point)
        {
            if (!_started)
            {
                _started = true;
                _lastVertex = new Vertex(1,point);
                _newPolygon = new Polygon(Color.MidnightBlue);
                _newPolygon.Vertices.Add(_lastVertex);
//                DrawDot(point, Color.Crimson);
                CommitDraw();
                return;
            }
            
            //Checking if it's time to end Cycle
            var firstVertex = _newPolygon.Vertices[0];
            var length = point.Length(firstVertex.Point);
            Console.WriteLine(length);
            if (length < 15)
            {
                if (_newPolygon.Vertices.Count >= 3)
                {
                    _started = false;
                    var lastEdgePoints = GetLine(firstVertex.Point.X, firstVertex.Point.Y, _lastVertex.Point.X, _lastVertex.Point.Y);
                    Edge lastEdge = new Edge(_lastVertex.Id, lastEdgePoints);
                    _lastVertex.Edges[1] = firstVertex.Edges[0] = lastEdge;
                    lastEdge.Vertices[0] = _lastVertex;
                    lastEdge.Vertices[1] = firstVertex;
                    
                    _newPolygon.Edges.Add(lastEdge);
                    _polygons.Add(_newPolygon);
                    
                    DrawPoints(lastEdge.Points,Color.MidnightBlue);
                    CommitDraw();
                }
                
                return;
            }
            
            //When it is time time to add not-start vertex
            var newVertex = new Vertex(_lastVertex.Id + 1, point);
            var edgePoints = GetLine(_lastVertex.Point.X, _lastVertex.Point.Y, point.X, point.Y);
            Edge newEdge = new Edge(_lastVertex.Id, edgePoints);
            
            _lastVertex.Edges[1] = newVertex.Edges[0] = newEdge;
            newEdge.Vertices[0] = _lastVertex;
            newEdge.Vertices[1] = newVertex;
            
            _newPolygon.Vertices.Add(newVertex);
            _newPolygon.Edges.Add(newEdge);

            //Drawing line and point
//            DrawDot(point, Color.Crimson);
            DrawPoints(newEdge.Points,Color.MidnightBlue);
            CommitDraw();

            _lastVertex = newVertex;
        }
    }
}