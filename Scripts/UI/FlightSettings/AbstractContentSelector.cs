using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.FlightSettings;

public abstract partial class AbstractContentSelector<T> : Control where T : ContentManagement.ContentItem
{
    public T SelectedItem
    {
        get
        {
            return AvailableItems[selector.Selected];
        }
    }

    private static Vector2 expectedTextureSize = new Vector2(1280, 720);
    private static Texture2D noThumbnailTexture = ResourceLoader.Load<Texture2D>("res://Art/NoThumbnail.png");

    [Export] private OptionButton selector;
    [Export] private TextureRect thumbnail;
    [Export] private RichTextLabel mainText;
    public List<T> AvailableItems
    {
        get
        {
            return _availableItems;
        }
        set
        {
            _availableItems = value;
            selector.Clear();
            foreach (var i in _availableItems) selector.AddItem(i.Name);
            _on_OptionButton_item_selected(0);
        }
    }
    private List<T> _availableItems = new();

    public override void _Ready()
    {
        _on_OptionButton_item_selected(0);
    }

    public void _on_OptionButton_item_selected(int index)
    {
        if (0 <= index && index < AvailableItems.Count)
        {
            selector.Disabled = false;

            thumbnail.Texture = LoadThumbnail();
            var customInfo = FormatCustomInfo();

            var formattedDateCreated = SelectedItem.DateCreated.ToString("dd MMMM yyyy");
            var formattedDateUpdated = SelectedItem.DateUpdated.ToString("dd MMMM yyyy");
            var sections = new List<string>()
            {
                $" By {SelectedItem.Author}    Version {SelectedItem.Version}    Created {formattedDateCreated}    Updated {formattedDateUpdated}",
                SelectedItem.Description,
                customInfo,
            };

            mainText.Text = String.Join("\n\n", sections.Select(x => x.Trim()).Where(x => x != ""));
        }
        else
        {
            thumbnail.Texture = null;
            mainText.Text = "";
            if (AvailableItems.Count == 0)
            {
                selector.Disabled = true;
                selector.Text = "No items found, the game is likely corrupted";
            }
        }
    }

    private Texture2D LoadThumbnail()
    {
        // todo: very weird bug: in v3.6 installed from aur, the failed loading thumbnail line is not displayed more than once,
        // unless a print is put before the following line
        var texture = ResourceLoader.Load<Texture2D>(SelectedItem.GetThumbnailPath());
        if (texture == null)
        {
            Utils.LogError($"Failed loading thumbnail from {SelectedItem.GetThumbnailPath()}", this);
            texture = noThumbnailTexture;
        }
        else if (texture.GetSize() != expectedTextureSize)
            Utils.LogError($"Thumbnail {SelectedItem.GetThumbnailPath()} was of size {texture.GetSize()} instead of {expectedTextureSize}", this);
        return texture;
    }

    public void SelectItem(int index)
    {
        if (index >= AvailableItems.Count || index < 0)
        {
            Utils.LogError($"Requested to select item at index {index} but there are only {AvailableItems.Count}", this);
            return;
        }

        selector.Select(index);
        _on_OptionButton_item_selected(index);
    }

    protected abstract string FormatCustomInfo();
}