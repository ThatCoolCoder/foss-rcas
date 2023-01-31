using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private RichTextLabel mainText;
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
            mainText = GetNode<RichTextLabel>("VBoxContainer/RichTextLabel");
        }
        public void _on_OptionButton_item_selected(int index)
        {
            thumbnail.Texture = ResourceLoader.Load<Texture>(SelectedItem.GetThumbnailPath());

            var customInfo = FormatCustomInfo();

            var formattedCredits = String.Join("\n", SelectedItem.Credits.Select(p => $"  {p.Key}: {p.Value}"));
            if (formattedCredits != "") formattedCredits = "Credits:\n" + formattedCredits;

            var formattedDate = SelectedItem.DateUpdated.ToString("dd MMMM yyyy");
            var sections = new List<string>()
            {
                $" By {SelectedItem.Author}\tVersion {SelectedItem.Version}\tUpdated {formattedDate}",
                SelectedItem.Description,
                customInfo,
                formattedCredits
            };

            mainText.BbcodeText = String.Join("\n\n", sections.Select(x => x.Trim()).Where(x => x != ""));

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
}