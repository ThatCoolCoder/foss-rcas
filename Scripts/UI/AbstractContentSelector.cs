using Godot;
using System;
using System.Collections.Generic;

namespace UI
{
    public abstract class AbstractContentSelector<T> : Control where T : ContentManagement.ContentItem
    {
        public T SelectedItem
        {
            get
            {
                return AvailableItems[selector.Selected];
            }
        }

        private OptionButton selector;
        private TextureRect thumbnail;
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
            selector = GetNode<OptionButton>("VBoxContainer/OptionButton");
            thumbnail = GetNode<TextureRect>("VBoxContainer/Image");
        }
        public void _on_OptionButton_item_selected(int index)
        {
            thumbnail.Texture = ResourceLoader.Load<Texture>(SelectedItem.GetThumbnailPath());
        }

        public abstract string FormatCustomInfo();
    }
}