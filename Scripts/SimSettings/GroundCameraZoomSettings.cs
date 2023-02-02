
namespace SimSettings
{
    public class GroundCameraZoomSettings
    {
        public bool Enabled { get; set; } = true;
        public float BaseFov { get; set; } = 70;
        public float StartDist { get; set; } = 40; // Maximum distance that plane can still be seen with base FOV
        // Rate of zoom compared to the "perfect rate". If this wasn't present, it would zoom perfectly and keep the plane the same size forever.
        // However, that would make judging distance difficult, so by setting this to something less than 1, it gives the best of both worlds
        public float Factor { get; set; } = 0.5f;
    }
}