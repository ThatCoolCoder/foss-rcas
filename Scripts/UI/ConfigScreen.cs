using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UI
{
    public class ConfigScreen : Control
    {
        private OptionButton aircraftSelector;
        private TextureRect aircraftThumbnail;
        private List<ContentManagement.Aircraft> availableAircraft = new();

        private OptionButton locationSelector;
        private TextureRect locationThumbnail;
        private List<ContentManagement.Location> availableLocations = new();

        public override void _Ready()
        {
            aircraftSelector = GetNode<OptionButton>("MarginContainer/VBoxContainer/HSplitContainer/AircraftSelector/OptionButton");
            aircraftThumbnail = GetNode<TextureRect>("MarginContainer/VBoxContainer/HSplitContainer/AircraftSelector/Image");

            locationSelector = GetNode<OptionButton>("MarginContainer/VBoxContainer/HSplitContainer/LocationSelector/OptionButton");
            locationThumbnail = GetNode<TextureRect>("MarginContainer/VBoxContainer/HSplitContainer/LocationSelector/Image");

            UpdateAvailableContent();
        }

        private void UpdateAvailableContent()
        {
            (availableAircraft, _) = ContentManagement.ContentLoader.FindContent("res://Scenes/Aircraft/");
            foreach (var a in availableAircraft) aircraftSelector.AddItem(a.Name);
            _on_AircraftSelector_item_selected(0);

            (_, availableLocations) = ContentManagement.ContentLoader.FindContent("res://Scenes/Locations/");
            foreach (var l in availableLocations) locationSelector.AddItem(l.Name);
            _on_LocationSelector_item_selected(0);
        }

        public void _on_AircraftSelector_item_selected(int index)
        {
            aircraftThumbnail.Texture = ResourceLoader.Load<Texture>(availableAircraft[index].GetThumbnailPath());
        }

        public void _on_LocationSelector_item_selected(int index)
        {
            locationThumbnail.Texture = ResourceLoader.Load<Texture>(availableLocations[index].GetThumbnailPath());
        }

        public void _on_BackButton_pressed()
        {
            GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
        }

        public void _on_PlayButton_pressed()
        {
            var location = ResourceLoader.Load<PackedScene>(availableLocations[locationSelector.Selected].GetScenePath()).Instance<Location>();
            var aircraft = ResourceLoader.Load<PackedScene>(availableAircraft[aircraftSelector.Selected].GetScenePath()).Instance<RigidBody>();
            location.AddChild(aircraft);
            location.Aircraft = aircraft;

            var root = GetTree().Root;
            root.RemoveChild(this);
            root.AddChild(location);
            QueueFree();
        }
    }
}