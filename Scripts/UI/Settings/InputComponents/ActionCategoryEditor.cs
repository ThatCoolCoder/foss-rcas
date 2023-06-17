using Godot;
using System;
using System.Collections.Generic;

namespace UI.Settings.InputComponents
{
    using SimInput;

    public class ActionCategoryEditor : Misc.CollapsibleMenu
    {
        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ActionCategoryEditor.tscn");

        private int categoryIndex;

        public ActionCategoryEditor Config(Node parent, string name, int _categoryIndex)
        {
            if (parent != null) parent.AddChild(this);

            Title = name;

            categoryIndex = _categoryIndex;

            SettingsScreen.OnSettingsChanged += OnSettingsChanged;

            return this;
        }

        private void OnSettingsChanged()
        {
            UpdateChildren();
        }

        private void UpdateChildren()
        {
            var category = SettingsScreen.NewSettings.InputMap.ActionCategories[categoryIndex];

            var holder = GetNode<Control>("MarginContainer/MaxSizeContainer/VBoxContainer/ActionList");

            foreach (var child in holder.GetChildNodeList()) child.QueueFree();

            foreach (var action in category.Actions)
            {
                ActionPreview.Scene.Instance<ActionPreview>().Config(holder, action);
            }
            UpdateLayout();
        }

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }
    }
}
