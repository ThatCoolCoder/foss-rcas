using Godot;
using System;

namespace UI.Settings
{

    public class InputChannelEditor : Misc.CollapsibleMenu
    {
        private int channelIndex;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/InputChannelEditor.tscn");

        // private VBoxContainer 

        public InputChannelEditor Config(Node parent, string name, int _channelIndex)
        {
            if (parent != null) parent.AddChild(this);
            channelIndex = _channelIndex;

            Title = name;

            SettingsScreen.OnSettingsChanged += OnSettingsChanged;

            return this;
        }

        public override void _Ready()
        {
            base._Ready();
        }

        private void OnSettingsChanged()
        {
            UpdateChildren();
        }

        private void UpdateChildren()
        {
            var channel = SettingsScreen.NewSettings.InputMap.Channels[channelIndex];

            var holder = GetNode<Control>("MarginContainer/MaxSizeContainer/VBoxContainer/MappingList");

            foreach (var child in holder.GetChildNodeList()) child.QueueFree();

            foreach (var mapping in channel.Mappings)
            {
                ControlMappingPreview.Scene.Instance<ControlMappingPreview>().Config(holder, mapping, DeleteControlMapping);
            }
            UpdateLayout();
        }

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }

        private void _on_NewMappingButton_pressed()
        {

        }

        private void DeleteControlMapping(SimInput.IControlMapping mapping)
        {
            SettingsScreen.NewSettings.InputMap.Channels[channelIndex].Mappings.Remove(mapping);
            UpdateChildren();
        }
    }
}
