using ShapeDrafter.Figures;

namespace ShapeDrafter.Models
{
    public class Scene
    {
        public Figure[] Figures;
        public Camera[] Cameras;
        public Light[] Lights;

        public Scene(Figure[] figures, Camera[] cameras, Light[] lights)
        {
            Figures = figures;
            Cameras = cameras;
            Lights = lights;
        }

        public Scene()
        {
        }
    }
}