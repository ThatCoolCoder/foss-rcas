using Godot;
using System;

namespace UI.Settings
{
    public class InputTab : Control
    {
        public override void _Ready()
        {
            var holder = GetNode<VBoxContainer>("VBoxContainer");
            var mappings = SimSettings.Settings.Current.InputMap.Channels;
            for (int i = 0; i < mappings.Count; i++)
            {
                InputChannelEditor.Scene.Instance<InputChannelEditor>().Config(holder, mappings[i].Name.Capitalize(), i);
            }
        }
    }
}