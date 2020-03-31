namespace ShapeDrafter
{
    public static class GlobalOptions
    {
        public static bool Perspective = true;
        public static bool Filling = true;
        public static bool BackFaceCulling = true;
        public static bool ZBuffering = true;
    }
}
            
            // List<Vertex> allVertices = new List<Vertex>();
            // foreach (var triangle in worldTriangles)
            //      allVertices.AddRange(Renderer.Rasterize(triangle));
            // var texels = allVertices.Select(v => Lightning.CalculateTexel(v));
            // DrawTexels(texels);


            //Rasterize it, most expensive operation
            // List<Texel>[] texelsArray = new List<Texel>[worldTriangles.Count];
            // Parallel.For(0, texelsArray.Length, i =>
            // {
            //     var tri = worldTriangles[i];
            //     var vertices = Renderer.Rasterize(tri);
            //     texelsArray[i] = vertices.Select(v => Lightning.CalculateTexel(v)).ToList();
            // });

            // lock (_frameLoad)
            // {
            //     if (_frameLoad._loading)
            //         return;
            //     _frameLoad._loading = true;
            // }
