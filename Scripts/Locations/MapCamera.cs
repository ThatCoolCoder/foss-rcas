using Godot;
using System;

namespace Locations;

public partial class MapCamera : Camera3D
{
    private Vector3 velocity; // (in local space)
    [Export] public float Height { get; set; } = 50;
    [Export] public float MaxSpeed { get; set; } = 30;
    [Export] public float Acceleration { get; set; } = 120;
    [Export] public float MinX { get; set; } = -100;
    [Export] public float MaxX { get; set; } = 100;
    [Export] public float MinZ { get; set; } = -100;
    [Export] public float MaxZ { get; set; } = 100;

    public override void _Ready()
    {
        var dist = Height / Mathf.Tan(-Rotation.X);
        GD.Print(dist);
        var pos = new Vector3(0, Height, dist).Rotated(Vector3.Up, Rotation.Y);
        Position = pos;
    }

    public override void _Process(double delta)
    {
        if (Current)
        {
            KeyboardMovement(delta);
        }
    }

    private void KeyboardMovement(double delta)
    {
        var fdelta = (float)delta;
        var crntAcceleration = new Vector3(SimInput.Manager.GetActionValue("camera/move_left_right"),
            0,
            -SimInput.Manager.GetActionValue("camera/move_backward_forward")) * Acceleration;

        velocity += crntAcceleration * fdelta;
        velocity.X = Mathf.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
        velocity.Z = Mathf.Clamp(velocity.Z, -MaxSpeed, MaxSpeed);

        if (crntAcceleration.X == 0) velocity.X = Utils.ConvergeValue(velocity.X, 0, Acceleration * fdelta);
        if (crntAcceleration.Z == 0) velocity.Z = Utils.ConvergeValue(velocity.Z, 0, Acceleration * fdelta);

        Position += velocity.Rotated(Vector3.Up, Rotation.Y) * fdelta;

        Position = Position.WithX(Mathf.Clamp(Position.X, MinX, MaxX)).WithZ(Mathf.Clamp(Position.Z, MinZ, MaxZ));
    }
}
