using Godot;
using System;

public class GroundCamera : Camera
{
    [Export] public bool Enabled { get; set; } = true;
    [Export] public NodePath TargetPath { get; set; }

    [Export] public float BaseFov { get; set; } = 70;
    [Export] public bool ZoomEnabled { get; set; } = true;
    [Export] public float ZoomStartDist { get; set; } = 40; // Maximum distance that plane can still be seen with base FOV
    // Rate of zoom compared to the "perfect rate". If this was set to 1 it would zoom perfectly and keep the plane the same size forever, but that would make judging distance very difficult.
    [Export] public float ZoomFactor { get; set; } = 0.5f;
    public Spatial Target { get; set; }

    public override void _Ready()
    {
        if (TargetPath != null) Target = GetNode<Spatial>(TargetPath);
    }

    public override void _Process(float delta)
    {
        if (Enabled && Target != null)
        {
            LookAt(Target.GlobalTranslation, Vector3.Up);

            if (ZoomEnabled)
            {
                var fovProportion = Mathf.Atan(1 / ZoomStartDist) / Mathf.Deg2Rad(BaseFov);

                var distance = Target.GlobalTranslation.DistanceTo(GlobalTranslation);
                var angle = Mathf.Atan(1 / (distance * ZoomFactor)) / fovProportion;
                angle = Mathf.Rad2Deg(angle);
                angle = Mathf.Clamp(angle, 1, BaseFov);

                Fov = angle;
            }
        }
    }
}
