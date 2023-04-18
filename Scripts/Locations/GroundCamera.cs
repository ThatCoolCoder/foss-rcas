using Godot;
using System;

namespace Locations
{
    public class GroundCamera : FlightCamera
    {
        [Export] public bool Enabled { get; set; } = true;
        [Export] public NodePath TargetPath { get; set; }
        public ZoomSettings CurrentZoomSettings;
        public Spatial Target { get; set; }

        public override void _Ready()
        {
            if (TargetPath != null) Target = GetNode<Spatial>(TargetPath);
            base._Ready();
        }

        public override void _Process(float delta)
        {
            if (Enabled && Target != null)
            {
                LookAt(Target.GlobalTranslation, Vector3.Up);

                if (CurrentZoomSettings.Enabled)
                {
                    var fovProportion = Mathf.Atan(1 / CurrentZoomSettings.StartDist) / Mathf.Deg2Rad(CurrentZoomSettings.BaseFov);

                    var distance = Target.GlobalTranslation.DistanceTo(GlobalTranslation);
                    var angle = Mathf.Atan(1 / (distance * CurrentZoomSettings.Factor)) / fovProportion;
                    angle = Mathf.Rad2Deg(angle);
                    angle = Mathf.Clamp(angle, 1, CurrentZoomSettings.BaseFov);

                    Fov = angle;
                }
            }
        }

        public class ZoomSettings
        {
            public bool Enabled { get; set; } = true;
            public float BaseFov { get; set; } = 70;
            public float StartDist { get; set; } = 40; // Maximum distance that plane can still be seen with base FOV
                                                       // Rate of zoom compared to the "perfect rate". If this wasn't present, it would zoom perfectly and keep the plane the same size forever.
                                                       // However, that would make judging distance difficult, so by setting this to something less than 1, it gives the best of both worlds
            public float Factor { get; set; } = 0.5f;
        }
    }
}
