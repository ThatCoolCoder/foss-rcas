using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UI
{
    public class ConfigScreen : Control
    {
        private AircraftSelector aircraftSelector;
        private LocationSelector locationSelector;

        public override void _Ready()
        {
            aircraftSelector = GetNode<AircraftSelector>("MarginContainer/VBoxContainer/HSplitContainer/AircraftSelector/");
            locationSelector = GetNode<LocationSelector>("MarginContainer/VBoxContainer/HSplitContainer/LocationSelector/");

            UpdateAvailableContent();

            TryLoadLastLoaded(SimSettings.Settings.Current.LastLoadedAircraft, aircraftSelector);
            TryLoadLastLoaded(SimSettings.Settings.Current.LastLoadedLocation, locationSelector);
        }

        private void UpdateAvailableContent()
        {
            var (availableAircraft, _) = ContentManagement.ContentLoader.FindContent("res://Scenes/Aircraft/");
            aircraftSelector.AvailableItems = availableAircraft;

            var (_, availableLocations) = ContentManagement.ContentLoader.FindContent("res://Scenes/Locations/");
            locationSelector.AvailableItems = availableLocations;
        }

        private void RememberLastLoadedContent()
        {
            SimSettings.Settings.Current.LastLoadedAircraft = aircraftSelector.SelectedItem.LoadedFrom;
            SimSettings.Settings.Current.LastLoadedLocation = locationSelector.SelectedItem.LoadedFrom;
            SimSettings.Settings.SaveCurrentSettings();
        }

        private void TryLoadLastLoaded<T>(string lastLoadedPath, AbstractContentSelector<T> selector) where T : ContentManagement.ContentItem
        {
            // Try loading the thing that was loaded the previous time

            var index = selector.AvailableItems.FindIndex(x => x.LoadedFrom == lastLoadedPath);
            if (index > 0) selector.SelectItem(index);
        }

        public void _on_BackButton_pressed()
        {
            GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
        }


        public void _on_PlayButton_pressed()
        {
            var location = ResourceLoader.Load<PackedScene>(locationSelector.SelectedItem.GetScenePath()).Instance<Location>();
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

            RememberLastLoadedContent();
        }
    }
}