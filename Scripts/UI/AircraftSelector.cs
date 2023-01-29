using Godot;
using System;
using System.Collections.Generic;

public class AircraftSelector : Control
{
    public ContentManagement.Aircraft SelectedAircraft
    {
        get
        {
            return AvailableAircraft[selector.Selected];
        }
    }

    private OptionButton selector;
    private TextureRect thumbnail;
    public List<ContentManagement.Aircraft> AvailableAircraft
    {
        get
        {
            return _availableAircraft;
        }
        set
        {
            _availableAircraft = value;
            selector.Clear();
            foreach (var a in _availableAircraft) selector.AddItem(a.Name);
            _on_OptionButton_item_selected(0);
        }
    }
    private List<ContentManagement.Aircraft> _availableAircraft = new();

    public override void _Ready()
    {
        selector = GetNode<OptionButton>("VBoxContainer/OptionButton");
        thumbnail = GetNode<TextureRect>("VBoxContainer/Image");
    }
    public void _on_OptionButton_item_selected(int index)
    {
        thumbnail.Texture = ResourceLoader.Load<Texture>(SelectedAircraft.GetThumbnailPath());
    }
}
