using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;

namespace ShapeDrafter.Models
{
    public class Edge
    {
        public Edge(int id, List<Point> points)
        {
            Id = id;
            Points = points;
        }

        public int Id { get; set; }
        public List<Point> Points { get; set; }
        public Vertex[] Vertices { get; set; } = new Vertex[2];
    }
}