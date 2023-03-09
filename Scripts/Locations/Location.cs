using Godot;
using System;

public class Location : Spatial
{
    public RigidBody Aircraft { get; set; }

    public AircraftLauncher.LauncherSettings LauncherSettings { get; set; } = null;

    private AircraftLauncher launcher;

    public override void _Ready()
    {
        var camera = GetNode<GroundCamera>("Camera");
        camera.Target = Aircraft;
        camera.CurrentZoomSettings = SimSettings.Settings.Current.GroundCameraZoom;
        launcher = GetNode<AircraftLauncher>("AircraftLauncher");
        Reset();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("reset")) Reset();
        if (Input.IsActionJustPressed("launch")) launcher.Launch();
    }

    private void Reset()
    {
        var needsLauncher = LauncherSettings != null;

        if (needsLauncher)
        {
            launcher.Transform = GetNode<Spatial>("StartLocation").Transform;
            launcher.Settings = LauncherSettings;
            launcher.SetTarget(Aircraft);
        }
        else
        {
            Aircraft.LinearVelocity = Vector3.Zero;
            Aircraft.AngularVelocity = Vector3.Zero;
            Aircraft.Transform = GetNode<Spatial>("StartLocation").Transform;
        }
    }
}
