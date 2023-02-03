using Godot;
using System;

namespace UI.Settings
{
    public class InputTab : Control
    {
        public override void _Ready()
        {
            var holder = GetNode<VBoxContainer>("VBoxContainer");
            var mappings = SimSettings.Settings.Current.InputMap.AxisMappings;
            for (int i = 0; i < mappings.Count; i++)
            {
                AxisEditor.Scene.Instance<AxisEditor>().Config(holder, mappings[i].Name.Capitalize(), i);
            }
        }
    }
}