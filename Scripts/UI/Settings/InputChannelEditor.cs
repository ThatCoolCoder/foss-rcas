using Godot;
using System;

namespace UI.Settings
{

    public class InputChannelEditor : Misc.CollapsibleMenu
    {
        private int channelIndex;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputChannelEditor.tscn");

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

            foreach (var child in this.GetChildNodeList())
            {
                if (child.GetIndex() != 0) child.QueueFree();
            }

            foreach (var mapping in channel.Mappings)
            {
                if (mapping is SimInput.AxisControlMapping axisControlMapping)
                {
                    ControlMappingEditor.Scene.Instance<ControlMappingEditor>().Config(this, axisControlMapping);
                }
            }
            UpdateLayout();
        }

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }
    }
}
