using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UI.FlightSettings
{
    public class FlightSettingsScreen : Control
    {
        private AircraftSelector aircraftSelector;
        private LocationSelector locationSelector;
        private PackedScene orbitCameraScene = ResourceLoader.Load<PackedScene>("res://Scenes/Aircraft/Common/OrbitCamera.tscn");

        public override void _Ready()
        {
            aircraftSelector = GetNode<AircraftSelector>("MarginContainer/VBoxContainer/HSplitContainer/AircraftSelector");
            locationSelector = GetNode<LocationSelector>("MarginContainer/VBoxContainer/HSplitContainer/LocationSelector");

            UpdateAvailableContent();

            // Try load the most recently used content
            TrySelectFromPath(SimSettings.Settings.Current.Misc.LastLoadedAircraft, aircraftSelector);
            TrySelectFromPath(SimSettings.Settings.Current.Misc.LastLoadedLocation, locationSelector);
        }

        private void UpdateAvailableContent()
        {
            var (availableAircraft, availableLocations) = ContentManagement.Loader.FindContent(SimSettings.Settings.Current.Misc.AddOnRepositoryPath);
            availableAircraft.AddRange(ContentManagement.Loader.FindContent(ContentManagement.Repositories.BaseAircraft).Item1);
            availableLocations.AddRange(ContentManagement.Loader.FindContent(ContentManagement.Repositories.BaseLocations).Item2);

            aircraftSelector.AvailableItems = availableAircraft;
            locationSelector.AvailableItems = availableLocations;
        }

        private void RememberLastLoadedContent()
        {
            SimSettings.Settings.Current.Misc.LastLoadedAircraft = aircraftSelector.SelectedItem.LoadedFrom;
            SimSettings.Settings.Current.Misc.LastLoadedLocation = locationSelector.SelectedItem.LoadedFrom;
            SimSettings.Settings.SaveCurrent();
        }

        private void TrySelectFromPath<T>(string lastLoadedPath, AbstractContentSelector<T> selector) where T : ContentManagement.ContentItem
        {
            // Try selecting the item that was loaded from a specific path

            var index = selector.AvailableItems.FindIndex(x => x.LoadedFrom == lastLoadedPath);
            if (index > 0) selector.SelectItem(index);
        }

        public void _on_BackButton_pressed()
        {
            GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
        }


        public void _on_PlayButton_pressed()
        {
            RememberLastLoadedContent();

            var location = ResourceLoader.Load<PackedScene>(locationSelector.SelectedItem.GetScenePath()).Instance<Locations.Location>();
            if (aircraftSelector.SelectedItem.NeedsLauncher)
            {
                location.LauncherSettings = new()
                {
                    Speed = aircraftSelector.SelectedItem.LauncherSpeed,
                    Height = aircraftSelector.SelectedItem.LauncherHeight,
                    Angle = Mathf.Deg2Rad(aircraftSelector.SelectedItem.LauncherAngleDegrees),
                };
            }

            var aircraft = ResourceLoader.Load<PackedScene>(aircraftSelector.SelectedItem.GetScenePath()).Instance<RigidBody>();

            location.AddChild(aircraft);
            location.Aircraft = aircraft;

            var root = GetTree().Root;
            var tree = GetTree();
            root.RemoveChild(this);
            root.AddChild(location);
            tree.CurrentScene = location;
            QueueFree();

            // Add default onboard cameras (need to do it after putting the plane in the world, so that they can figure out their orientation)
            {
                var size = Mathf.Max(aircraftSelector.SelectedItem.Length, aircraftSelector.SelectedItem.WingSpan);
                var freeCamera = orbitCameraScene.Instance<Aircraft.OrbitCamera>();
                freeCamera.Name = "Free";
                freeCamera.OrbitRadius = size;
                freeCamera.RotateWithAircraft = false;
                aircraft.AddChild(freeCamera);

                var lockedCamera = orbitCameraScene.Instance<Aircraft.OrbitCamera>();
                lockedCamera.Name = "Locked";
                lockedCamera.OrbitRadius = size;
                lockedCamera.RotateWithAircraft = true;
                aircraft.AddChild(lockedCamera);
            }
        }
    }
}