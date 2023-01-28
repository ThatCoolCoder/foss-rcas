using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UI
{
    public class ConfigScreen : Control
    {
        private OptionButton locationSelector;
        private TextureRect locationThumbnail;
        private List<string> availableLocations = new();

        private OptionButton aircraftSelector;
        private TextureRect aircraftThumbnail;
        private List<string> availableAircraft = new();

        public override void _Ready()
        {
            locationSelector = GetNode<OptionButton>("MarginContainer/VBoxContainer/HSplitContainer/LocationSelector/OptionButton");
            locationThumbnail = GetNode<TextureRect>("MarginContainer/VBoxContainer/HSplitContainer/LocationSelector/Image");
            UpdateAvailableLocations();

            aircraftSelector = GetNode<OptionButton>("MarginContainer/VBoxContainer/HSplitContainer/AircraftSelector/OptionButton");
            aircraftThumbnail = GetNode<TextureRect>("MarginContainer/VBoxContainer/HSplitContainer/AircraftSelector/Image");
            UpdateAvailableAircraft();
        }

        private void UpdateAvailableLocations()
        {
            // todo: better code for removing extension
            availableLocations = Utils.GetItemsInDirectory("res://Scenes/Locations/")
                .Select(Utils.SplitAtExtension)
                .Where(x => x.Item2 == "tscn")
                .Select(x => x.Item1)
                .ToList();
            foreach (var l in availableLocations) locationSelector.AddItem(l);
            _on_LocationSelector_item_selected(0);
        }

        private void UpdateAvailableAircraft()
        {
            availableAircraft = Utils.GetItemsInDirectory("res://Scenes/Aircraft/")
                .Select(Utils.SplitAtExtension)
                .Where(x => x.Item2 == "tscn")
                .Select(x => x.Item1)
                .ToList();
            foreach (var a in availableAircraft) aircraftSelector.AddItem(a);
            _on_AircraftSelector_item_selected(0);
        }

        public void _on_LocationSelector_item_selected(int index)
        {
            locationThumbnail.Texture = ResourceLoader.Load<Texture>($"res://Scenes/Locations/{availableLocations[index]}.png");
        }

        public void _on_AircraftSelector_item_selected(int index)
        {
            aircraftThumbnail.Texture = ResourceLoader.Load<Texture>($"res://Scenes/Aircraft/{availableAircraft[index]}.png");
        }

        public void _on_PlayButton_pressed()
        {
            var location = ResourceLoader.Load<PackedScene>($"res://Scenes/Locations/{availableLocations[locationSelector.Selected]}.tscn").Instance<Location>();
            var aircraft = ResourceLoader.Load<PackedScene>($"res://Scenes/Aircraft/{availableAircraft[aircraftSelector.Selected]}.tscn").Instance<RigidBody>();
            location.AddChild(aircraft);
            location.Aircraft = aircraft;

            var root = GetTree().Root;
            root.RemoveChild(this);
            root.AddChild(location);
            QueueFree();
        }
    }
}