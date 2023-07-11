using Godot;
using System;

namespace Locations;

public partial class FreeCamera : BasicFlightCamera
{
    private Vector3 velocity; // (in local space)
    [Export] public float MaxSpeed { get; set; } = 10;
    [Export] public float Acceleration { get; set; } = 40;

    [Export] public float MaxAngularSpeedDegrees { get; set; } = 120;
    [Export] public float AngularAccelerationDegrees { get; set; } = 480;
    [Export] public float MouseSensitivity { get; set; } = 1;

    private ViewRotationManager rotationManager = new();

    public override void _Ready()
    {
        rotationManager.Target = this;
        rotationManager.MaxAngularSpeedDegrees = MaxAngularSpeedDegrees;
        rotationManager.AngularAccelerationDegrees = AngularAccelerationDegrees;
        rotationManager.MouseSensitivity = MouseSensitivity;

        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (Current)
        {
            KeyboardMovement(delta);

            rotationManager.Update(delta);
        }
    }

    private void KeyboardMovement(double delta)
    {
        var fdelta = (float)delta;
        var crntAcceleration = new Vector3(SimInput.Manager.GetActionValue("camera/move_left_right"),
            SimInput.Manager.GetActionValue("camera/move_down_up"),
            -SimInput.Manager.GetActionValue("camera/move_backward_forward")) * Acceleration;

        velocity += crntAcceleration * fdelta;
        velocity = velocity.LimitLength(MaxSpeed);

        if (crntAcceleration.X == 0) velocity.X = Utils.ConvergeValue(velocity.X, 0, Acceleration * fdelta);
        if (crntAcceleration.Y == 0) velocity.Y = Utils.ConvergeValue(velocity.Y, 0, Acceleration * fdelta);
        if (crntAcceleration.Z == 0) velocity.Z = Utils.ConvergeValue(velocity.Z, 0, Acceleration * fdelta);

        Position += Transform.Basis * velocity * fdelta;
    }

    public override void _UnhandledInput(InputEvent _event)
    {
        if (!Current) return;

        rotationManager.UnhandledInput(_event);
    }

    public override void BeforeActivated()
    {
        var camera = GetViewport().GetCamera3D();
        GlobalPosition = camera.GlobalPosition;

        rotationManager.SetPanAndTilt(camera.GlobalRotation.X, camera.GlobalRotation.Y);
    }
}
