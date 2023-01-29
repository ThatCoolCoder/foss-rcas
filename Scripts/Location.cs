using Godot;
using System;

public class Location : Spatial
{
    public RigidBody Aircraft { get; set; }

    public override void _Ready()
    {
        GetNode<AngleTracker>("CameraHolder").Target = Aircraft;
        Reset();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("reset"))
        {
            Reset();
        }
    }

    private void Reset()
    {
        Aircraft.LinearVelocity = Vector3.Zero;
        Aircraft.AngularVelocity = Vector3.Zero;
        Aircraft.Transform = GetNode<Spatial>("StartLocation").Transform;
    }
}
