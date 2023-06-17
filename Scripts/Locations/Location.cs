using Godot;
using System;

namespace Locations
{
    public class Location : Spatial
    {
        public RigidBody Aircraft { get; set; }

        public AircraftLauncher.LauncherSettings LauncherSettings { get; set; } = null;

        private AircraftLauncher launcher;

        public override void _Ready()
        {
            var camera = GetNode<WalkingGroundCamera>("Camera");
            SimSettings.Settings.Current.ApplyToViewport(GetViewport());
            SimInput.Manager.Instance.LoadInputMap(SimSettings.Settings.Current.InputMap);
            camera.Target = Aircraft;
            camera.CurrentZoomSettings = SimSettings.Settings.Current.GroundCameraZoom;
            launcher = GetNode<AircraftLauncher>("AircraftLauncher");
            Reset();
        }

        public override void _Process(float delta)
        {
            if (SimInput.Manager.IsActionJustPressed("gameplay/reset")) Reset();
            if (SimInput.Manager.IsActionJustPressed("gameplay/launch")) launcher.Launch();
        }

        private void Reset()
        {
            var needsLauncher = LauncherSettings != null;

            if (needsLauncher)
            {
                launcher.GlobalTransform = GetNode<Spatial>("StartLocation").GlobalTransform;
                launcher.Settings = LauncherSettings;
                launcher.SetTarget(Aircraft);
            }
            else
            {
                Aircraft.LinearVelocity = Vector3.Zero;
                Aircraft.AngularVelocity = Vector3.Zero;
                Aircraft.GlobalTransform = GetNode<Spatial>("StartLocation").GlobalTransform;
            }
        }
    }

}