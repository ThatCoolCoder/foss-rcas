using Godot;
using System;

namespace Aircraft;

public partial class OrbitCamera : Node3D, Locations.IFlightCamera
{
    [Export] public float OrbitRadius { get; set; } = 1;
    [Export] public float MaxAngularSpeedDegrees { get; set; } = 120;
    [Export] public float AngularAccelerationDegrees { get; set; } = 480;
    [Export] public float MouseSensitivity { get; set; } = 1;

    private Locations.ViewRotationManager rotationManager = new();

    [Export] public string ViewName { get; set; } = "Orbit";
    [Export] public bool RotateWithAircraft { get; set; } = false;

    private Camera3D camera;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");
        camera.Position = new Vector3(0, 0, OrbitRadius);
        if (!RotateWithAircraft) RotateY(GetParent<Node3D>().GlobalRotation.Y);
        Locations.CameraManager.instance.AddCamera(this);

        rotationManager.MaxAngularSpeedDegrees = MaxAngularSpeedDegrees;
        rotationManager.AngularAccelerationDegrees = AngularAccelerationDegrees;
        rotationManager.MouseSensitivity = MouseSensitivity;
    }

    public override void _ExitTree()
    {
        Locations.CameraManager.instance.RemoveCamera(this);
    }

    public override void _Process(double delta)
    {
        if (camera.Current)
        {
            rotationManager.Update(delta);
            var (pan, tilt) = rotationManager.GetPanAndTilt();
            var angle = new Vector3(pan, tilt, 0);
            if (RotateWithAircraft) Rotation = angle;
            else GlobalRotation = angle;
        }
    }

    public override void _UnhandledInput(InputEvent _event)
    {
        if (camera.Current)
        {
            rotationManager.UnhandledInput(_event);
        }
    }

    public void Activate()
    {
        camera.Current = true;
    }

    public void Deactivate()
    {
        camera.Current = false;
    }
}