using System.Collections.Generic;

namespace ShapeDrafter.Models
{
    public class ParallelEdges
    {
        public List<int> Edges { get; set; }
        public List<int> Vertices { get; set; }
        public bool Parallel { get; set; }
    }
}