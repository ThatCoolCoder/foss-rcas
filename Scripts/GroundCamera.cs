using Godot;
using System;

public class GroundCamera : Camera
{
    [Export] public bool Enabled { get; set; } = true;
    [Export] public NodePath TargetPath { get; set; }
    public SimSettings.GroundCameraZoomSettings ZoomSettings;
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

            if (ZoomSettings.Enabled)
            {
                var fovProportion = Mathf.Atan(1 / ZoomSettings.StartDist) / Mathf.Deg2Rad(ZoomSettings.BaseFov);

                var distance = Target.GlobalTranslation.DistanceTo(GlobalTranslation);
                var angle = Mathf.Atan(1 / (distance * ZoomSettings.Factor)) / fovProportion;
                angle = Mathf.Rad2Deg(angle);
                angle = Mathf.Clamp(angle, 1, ZoomSettings.BaseFov);

                Fov = angle;
            }
        }
    }
}
