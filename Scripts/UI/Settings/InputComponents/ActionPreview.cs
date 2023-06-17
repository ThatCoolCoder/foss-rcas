using Godot;
using System;
using System.Collections.Generic;

using SimInput;

namespace UI.Settings.InputComponents
{
    public class ActionPreview : Control
    {
        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ActionPreview.tscn");

        private SimInput.InputAction action;
        private HBoxContainer mappingHolder;

        public ActionPreview Config(Node parent, SimInput.InputAction _action)
        {
            action = _action;
            if (parent != null) parent.AddChild(this);

            return this;
        }

        public override void _Ready()
        {
            GetNode<Label>("Label").Text = action.DisplayName;
            mappingHolder = GetNode<HBoxContainer>("MappingHolder");
            HintTooltip = action.Description;

            SettingsScreen.OnSettingsChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged()
        {
            UpdateMappings();
        }

        private void UpdateMappings()
        {
            foreach (var child in mappingHolder.GetChildNodeList()) child.QueueFree();

            foreach (var mapping in action.Mappings)
            {
                ControlMappingPreview.Scene.Instance<ControlMappingPreview>().Config(mappingHolder, mapping, DeleteControlMapping);
            }
        }

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }

        private void DeleteControlMapping(SimInput.IControlMapping mapping)
        {
            action.Mappings.Remove(mapping);
            UpdateMappings();
        }
    }
}
