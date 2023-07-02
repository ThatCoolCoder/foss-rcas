using Godot;
using System;
using System.Collections.Generic;

namespace UI.Settings.InputComponents
{
    using SimInput;

    public class ActionCategoryEditor : Misc.CollapsibleMenu
    {
        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ActionCategoryEditor.tscn");

        private InputActionCategory category;

        public ActionCategoryEditor Config(Node parent, string name, InputActionCategory _category)
        {
            if (parent != null) parent.AddChild(this);

            Title = name;

            category = _category;

            SettingsScreen.OnSettingsChanged += OnSettingsChanged;

            return this;
        }

        private void OnSettingsChanged()
        {
            UpdateChildren();
        }

        private void UpdateChildren()
        {
            var holder = GetNode<Control>("MarginContainer/VBoxContainer/ActionList");

            foreach (var child in holder.GetChildNodeList()) child.QueueFree();

            foreach (var action in category.Actions)
            {
                ActionPreview.Scene.Instance<ActionPreview>().Config(holder, action, category.Name + "/" + action.Name);
            }
            UpdateLayout();
        }

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }
    }
}
