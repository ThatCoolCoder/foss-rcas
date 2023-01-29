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
        }

        private void UpdateAvailableContent()
        {
            var (availableAircraft, _) = ContentManagement.ContentLoader.FindContent("res://Scenes/Aircraft/");
            aircraftSelector.AvailableAircraft = availableAircraft;

            var (_, availableLocations) = ContentManagement.ContentLoader.FindContent("res://Scenes/Locations/");
            locationSelector.AvailableLocations = availableLocations;
        }

        public void _on_BackButton_pressed()
        {
            GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
        }

        public void _on_PlayButton_pressed()
        {
            var location = ResourceLoader.Load<PackedScene>(locationSelector.SelectedLocation.GetScenePath()).Instance<Location>();
            var aircraft = ResourceLoader.Load<PackedScene>(aircraftSelector.SelectedAircraft.GetScenePath()).Instance<RigidBody>();
            location.AddChild(aircraft);
            location.Aircraft = aircraft;

            var root = GetTree().Root;
            root.RemoveChild(this);
            root.AddChild(location);
            QueueFree();
        }
    }
}