using System.Drawing;

namespace ShapeDrafter.Models
{
    public class Vertex
    {
        public Vertex(int id, Point point)
        {
            Id = id;
            Point = point;
        }

        public override string ToString()
        {
            return $"Location: {Point}, Id: {Id}, Edges: ({Edges[0].Id},{Edges[1].Id})";
        }

        public Edge[] Edges { get; set; } = new Edge[2];
        public int Id { get; set; }
        public Point Point { get; set; }
    }
}