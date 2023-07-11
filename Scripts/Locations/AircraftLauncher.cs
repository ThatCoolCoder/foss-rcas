using Godot;
using System;

namespace Locations;

public partial class AircraftLauncher : Node3D
{
    public partial class LauncherSettings
    {
        public float Height { get; set; } = 1.5f;
        public float Speed { get; set; } = 10;
        public float AngleDegrees { get; set; } = 0;
    }

    public LauncherSettings Settings { get; set; }
    public Vector3 PositionOffset { get; set; }
    private RigidBody3D target;
    private bool used = false;

    public void Reset(RigidBody3D newTarget)
    {
        target = newTarget;
        target.Freeze = true;
        used = false;

        target.LinearVelocity = Vector3.Zero;
        target.AngularVelocity = Vector3.Zero;
        target.GlobalPosition = GlobalPosition.WithY(GlobalPosition.Y + Settings.Height) + PositionOffset.Rotated(Vector3.Up, GlobalRotation.Y);
        target.Rotation = new Vector3(Mathf.DegToRad(Settings.AngleDegrees), Rotation.Y, 0);
    }

    public void Launch()
    {
        if (used || target == null) return;

        used = true;
        target.Freeze = false;
        target.LinearVelocity = (Vector3.Forward * Settings.Speed).Rotated(Vector3.Right, Mathf.DegToRad(Settings.AngleDegrees)).Rotated(Vector3.Up, Rotation.Y);
    }
}