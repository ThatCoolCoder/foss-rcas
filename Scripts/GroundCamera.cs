using Godot;
using System;

public class GroundCamera : Camera
{
    [Export] public bool Enabled { get; set; } = true;
    [Export] public NodePath TargetPath { get; set; }

    [Export] public float BaseFov { get; set; } = 70;
    [Export] public bool ZoomEnabled { get; set; } = true;
    [Export] public float ZoomStartDist { get; set; } = 30;
    [Export] public float ZoomFactor { get; set; } = 0.4f;
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
                var distance = Target.GlobalTranslation.DistanceTo(GlobalTranslation);
                var deltaDist = Mathf.Max(distance - ZoomStartDist, 0);
                Fov = Mathf.Max(BaseFov - deltaDist * ZoomFactor, 1);
            }
        }
    }
}
