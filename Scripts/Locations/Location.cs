using Godot;
using System;
using System.Collections.Generic;

namespace Locations;

public partial class Location : Node3D
{
    [Export] public UI.Apps.Management.AppManager UIAppManager { get; set; }
    [Export] public CameraManager CameraManager { get; set; }
    public Aircraft.Aircraft Aircraft { get; set; }
    public ContentManagement.Aircraft AircraftInfo { get; set; }
    public ContentManagement.Location LocationInfo { get; set; }
    public ContentManagement.AircraftSpawnPosition CrntSpawnPosition { get; set; }

    private const int maxSlowMotion = 64; // as in 1/64 speed
    private const int minSlowMotion = 1;
    private int slowMotionAmount = 1; // this becomes a fraction, eg a value of 2 equals 1/2 speed
    private int previousCameraIndex;
    [Export] private AircraftLauncher launcher;
    [Export] private GroundCamera groundCamera;
    private PackedScene orbitCameraScene = ResourceLoader.Load<PackedScene>("res://Scenes/Aircraft/Common/OrbitCamera.tscn");

    public override void _EnterTree()
    {
        // Some children may need this before ready, so do it here
        SimSettings.Settings.Current.ApplyToViewport(GetViewport() as SubViewport);
    }

    public override void _Ready()
    {
        if (SimSettings.Settings.Current.View.AdjustZoomForAircraftSize) groundCamera.ZoomDistMultiplier = AircraftInfo.WingSpan;
        groundCamera.CurrentZoomSettings = SimSettings.Settings.Current.View.GroundCameraZoom;

        UIAppManager.SetAvailableProfiles(SimSettings.Settings.Current.UIAppProfiles);

        SetupAircraft();
        Reset();
    }

    public override void _Process(double delta)
    {
        if (SimInput.Manager.IsActionJustPressed("gameplay/reset")) Reset();
        if (SimInput.Manager.IsActionJustPressed("gameplay/reload_aircraft")) ReloadAircraft();
        if (SimInput.Manager.IsActionJustPressed("gameplay/launch")) launcher.Launch();
        if (SimInput.Manager.IsActionJustPressed("gameplay/pause"))
        {
            GetTree().Paused = !GetTree().Paused;
            var text = GetTree().Paused ? "Paused" : "Unpaused";
            UI.NotificationManager.AddNotification(text, "time");
        }

        if (SimInput.Manager.IsActionJustPressed("gameplay/more_slow_motion")) AdjustSlowMotion(slowMotionAmount * 2);
        if (SimInput.Manager.IsActionJustPressed("gameplay/less_slow_motion")) AdjustSlowMotion(slowMotionAmount / 2);
        if (SimInput.Manager.IsActionJustPressed("gameplay/reset_slow_motion")) AdjustSlowMotion(1);
    }

    private void AdjustSlowMotion(int newSlowMotionAmount)
    {
        slowMotionAmount = Mathf.Clamp(newSlowMotionAmount, minSlowMotion, maxSlowMotion);
        Engine.TimeScale = 1f / slowMotionAmount;

        if (slowMotionAmount == 1) UI.NotificationManager.AddNotification("Normal speed", "time");
        else UI.NotificationManager.AddNotification($"1/{slowMotionAmount} speed", "time");
    }

    private void SetupAircraft()
    {
        launcher.Settings = new AircraftLauncher.LauncherSettings()
        {
            Speed = AircraftInfo.LauncherSpeed,
            Height = AircraftInfo.LauncherHeight,
            AngleDegrees = AircraftInfo.LauncherAngleDegrees
        };
        launcher.PositionOffset = AircraftInfo.PositionOffset;

        // Set up cameras
        var orbitRadius = Mathf.Max(AircraftInfo.Length, AircraftInfo.WingSpan);
        var freeCamera = orbitCameraScene.Instantiate<Aircraft.OrbitCamera>();
        freeCamera.ViewName = "Orbit - Unlocked";
        freeCamera.OrbitRadius = orbitRadius;
        freeCamera.RotateWithAircraft = false;
        Aircraft.AddChild(freeCamera);

        var lockedCamera = orbitCameraScene.Instantiate<Aircraft.OrbitCamera>();
        lockedCamera.ViewName = "Orbit - Locked";
        lockedCamera.OrbitRadius = orbitRadius;
        lockedCamera.RotateWithAircraft = true;
        Aircraft.AddChild(lockedCamera);

        groundCamera.Target = Aircraft;
        groundCamera.GlobalPosition = GetNode<Node3D>(CrntSpawnPosition.CameraPositionNodePath).GlobalPosition;
    }

    private void ReloadAircraft()
    {
        previousCameraIndex = CameraManager.ActiveCameraIndex;
        var oldAircraftConfig = Aircraft.Config;
        void DisplayReloadError(string message) => UI.NotificationManager.AddNotification($"Failed reloading aircraft: {message}", "aircraft_reload");

        var scene = ResourceLoader.Load<PackedScene>(AircraftInfo.LoadedFromWithoutExtension + ".tscn", cacheMode: ResourceLoader.CacheMode.Ignore);
        if (scene == null)
        {
            DisplayReloadError("file not found");
            return;
        }

        try
        {
            var instance = scene.Instantiate<Aircraft.Aircraft>();
            if (instance == null)
            {
                DisplayReloadError("instance did not appear");
                return;
            }
            Aircraft.QueueFree();
            Aircraft.Connect("tree_exited", new Callable(this, "ReactivatePreviousCamera")); // (need to do AFTER aircraft is actually deleted)
            Aircraft = instance;
            AddChild(Aircraft);
            Aircraft.Config = oldAircraftConfig;
        }
        catch (InvalidCastException)
        {
            DisplayReloadError("scene doesn't extend Aircraft");
            return;
        }

        SetupAircraft();
        Reset();
    }

    private void ReactivatePreviousCamera()
    {
        CameraManager.ActivateCamera(previousCameraIndex);
    }

    private void Reset()
    {
        var aircraftTransform = GetNode<Node3D>(CrntSpawnPosition.AircraftPositionNodePath).GlobalTransform;
        if (AircraftInfo.NeedsLauncher)
        {
            launcher.GlobalTransform = aircraftTransform;
            launcher.Reset(Aircraft);
        }
        else
        {
            Aircraft.LinearVelocity = Vector3.Zero;
            Aircraft.AngularVelocity = Vector3.Zero;
            Aircraft.GlobalTransform = aircraftTransform;
            Aircraft.GlobalPosition += aircraftTransform.Basis * AircraftInfo.PositionOffset;
        }
        Aircraft.Reset();
    }

    public override void _ExitTree()
    {
        // Make sure we reset time etc, since the menus don't appreciate being paused
        GetTree().Paused = false;
        Engine.TimeScale = 1;
    }

    private void _on_UIAppManager_ChangesSaved()
    {
        // (everything is a reference type in there, so no assigning needs to be done)
        SimSettings.Settings.SaveCurrent();
    }
}