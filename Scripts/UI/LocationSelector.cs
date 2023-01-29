using Godot;
using System;
using System.Collections.Generic;

public class LocationSelector : Control
{
    public ContentManagement.Location SelectedLocation
    {
        get
        {
            return AvailableLocations[selector.Selected];
        }
    }

    private OptionButton selector;
    private TextureRect thumbnail;
    public List<ContentManagement.Location> AvailableLocations
    {
        get
        {
            return _availableLocations;
        }
        set
        {
            _availableLocations = value;
            selector.Clear();
            foreach (var a in _availableLocations) selector.AddItem(a.Name);
            _on_OptionButton_item_selected(0);
        }
    }
    private List<ContentManagement.Location> _availableLocations = new();

    public override void _Ready()
    {
        selector = GetNode<OptionButton>("OptionButton");
        thumbnail = GetNode<TextureRect>("Image");
    }

    public void _on_OptionButton_item_selected(int index)
    {
        thumbnail.Texture = ResourceLoader.Load<Texture>(SelectedLocation.GetThumbnailPath());
    }
}
