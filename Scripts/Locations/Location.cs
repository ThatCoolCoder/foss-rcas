using Godot;
using System;

namespace Locations
{
    public class Location : Spatial
    {
        public RigidBody Aircraft { get; set; }
        public ContentManagement.Aircraft AircraftInfo { get; set; }
        public ContentManagement.Location LocationInfo { get; set; }
        public ContentManagement.AircraftSpawnPosition CrntSpawnPosition { get; set; }

        private AircraftLauncher launcher;
        private GroundCamera groundCamera;
        private PackedScene orbitCameraScene = ResourceLoader.Load<PackedScene>("res://Scenes/Aircraft/Common/OrbitCamera.tscn");

        public override void _Ready()
        {
            groundCamera = GetNode<GroundCamera>("GroundCamera");
            SimSettings.Settings.Current.ApplyToViewport(GetViewport());
            SimInput.Manager.Instance.LoadInputMap(SimSettings.Settings.Current.InputMap);
            groundCamera.Target = Aircraft;
            groundCamera.CurrentZoomSettings = SimSettings.Settings.Current.GroundCameraZoom;
            launcher = GetNode<AircraftLauncher>("AircraftLauncher");

            SetupAircraft();
            Reset();
        }

        private void SetupAircraft()
        {
            launcher.Settings = new AircraftLauncher.LauncherSettings()
            {
                Speed = AircraftInfo.LauncherSpeed,
                Height = AircraftInfo.LauncherHeight,
                AngleDegrees = AircraftInfo.LauncherAngleDegrees
            };

            // Set up cameras
            var orbitRadius = Mathf.Max(AircraftInfo.Length, AircraftInfo.WingSpan);
            var freeCamera = orbitCameraScene.Instance<Aircraft.OrbitCamera>();
            freeCamera.ViewName = "Orbit - Unlocked";
            freeCamera.OrbitRadius = orbitRadius;
            freeCamera.RotateWithAircraft = false;
            Aircraft.AddChild(freeCamera);

            var lockedCamera = orbitCameraScene.Instance<Aircraft.OrbitCamera>();
            lockedCamera.ViewName = "Orbit - Locked";
            lockedCamera.OrbitRadius = orbitRadius;
            lockedCamera.RotateWithAircraft = true;
            Aircraft.AddChild(lockedCamera);
        }

        public override void _Process(float delta)
        {
            if (SimInput.Manager.IsActionJustPressed("gameplay/reset")) Reset();
            if (SimInput.Manager.IsActionJustPressed("gameplay/launch")) launcher.Launch();
        }

        private void Reset()
        {
            var aircraftTransform = GetNode<Spatial>(CrntSpawnPosition.AircraftPositionNodePath).GlobalTransform;
            if (AircraftInfo.NeedsLauncher)
            {
                launcher.GlobalTransform = aircraftTransform;
            }
            else
            {
                Aircraft.LinearVelocity = Vector3.Zero;
                Aircraft.AngularVelocity = Vector3.Zero;
                Aircraft.GlobalTransform = aircraftTransform;
            }
            groundCamera.GlobalTranslation = GetNode<Spatial>(CrntSpawnPosition.CameraPositionNodePath).GlobalTranslation;
            launcher.Reset(Aircraft);
        }
    }

}