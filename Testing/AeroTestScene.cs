using Godot;
using System;

public class AeroTestScene : Spatial
{
    public override void _Ready()
    {
        SimSettings.Settings.LoadCurrent();
        SimInput.Manager.ApplyAxisMappings();
    }

    public override void _Process(float delta)
    {
        var n = GetNode<Spatial>("RigidBody/AeroSurface");
        if (Input.IsActionPressed("ui_up")) n.RotationDegrees = n.RotationDegrees.WithZ(n.RotationDegrees.z + delta * 10);
        if (Input.IsActionPressed("ui_down")) n.RotationDegrees = n.RotationDegrees.WithZ(n.RotationDegrees.z - delta * 10);
    }
}
