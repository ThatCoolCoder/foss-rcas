using Godot;
using System;

public class Location : Spatial
{
    public RigidBody Aircraft { get; set; }

    public override void _Ready()
    {
        GetNode<AngleTracker>("CameraHolder").Target = Aircraft;

        Aircraft.Transform = GetNode<Spatial>("StartLocation").Transform;
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("reset"))
        {
            Aircraft.LinearVelocity = Vector3.Zero;
            Aircraft.AngularVelocity = Vector3.Zero;

            _Ready();
        }
    }
}
