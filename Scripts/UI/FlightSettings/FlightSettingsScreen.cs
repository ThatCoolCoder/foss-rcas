using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UI.FlightSettings
{
    public partial class FlightSettingsScreen : Control
    {
        private AircraftSelector aircraftSelector;
        private LocationSelector locationSelector;

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
            ContentManagement.Loader.SearchForAddons(SimSettings.Settings.Current.Misc.AddonRepositoryPath);

            var (availableAircraft, availableLocations) = ContentManagement.Loader.FindContent(ContentManagement.Loader.AddonContentDirectory);
            availableAircraft.AddRange(ContentManagement.Loader.FindContent(ContentManagement.Repositories.BaseAircraft).Item1);
            availableLocations.AddRange(ContentManagement.Loader.FindContent(ContentManagement.Repositories.BaseLocations).Item2);

            aircraftSelector.AvailableItems = availableAircraft.OrderBy(x => x.Name).ToList();
            locationSelector.AvailableItems = availableLocations.OrderBy(x => x.Name).ToList();
        }

        private void RememberLastLoadedContent()
        {
            SimSettings.Settings.Current.Misc.LastLoadedAircraft = aircraftSelector.SelectedItem.LoadedFromWithoutExtension;
            SimSettings.Settings.Current.Misc.LastLoadedLocation = locationSelector.SelectedItem.LoadedFromWithoutExtension;
            SimSettings.Settings.SaveCurrent();
        }

        private void TrySelectFromPath<T>(string lastLoadedPath, AbstractContentSelector<T> selector) where T : ContentManagement.ContentItem
        {
            // Try selecting the item that was loaded from a specific path

            var index = selector.AvailableItems.FindIndex(x => x.LoadedFromWithoutExtension == lastLoadedPath);
            if (index > 0) selector.SelectItem(index);
        }

        public void _on_BackButton_pressed()
        {
            GetTree().ChangeSceneToFile("res://Scenes/UI/StartScreen.tscn");
        }


        public void _on_PlayButton_pressed()
        {
            RememberLastLoadedContent();

            var location = ResourceLoader.Load<PackedScene>(locationSelector.SelectedItem.GetScenePath()).Instantiate<Locations.Location>();
            location.LocationInfo = locationSelector.SelectedItem;

            var aircraft = ResourceLoader.Load<PackedScene>(aircraftSelector.SelectedItem.GetScenePath()).Instantiate<RigidBody3D>();
            location.AircraftInfo = aircraftSelector.SelectedItem;

            location.AddChild(aircraft);
            location.Aircraft = aircraft;
            location.CrntSpawnPosition = locationSelector.SelectedItem.SpawnPositions.First();
            GD.Print(location.CrntSpawnPosition.Name);

            // switch scenes, done manually because we needed to set the values above
            var root = GetTree().Root;
            var tree = GetTree();
            root.RemoveChild(this);
            root.AddChild(location);
            tree.CurrentScene = location;
            QueueFree();
        }
    }
}