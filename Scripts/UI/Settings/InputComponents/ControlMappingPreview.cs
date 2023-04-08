using Godot;
using System;
using System.Collections.Generic;

using SimInput;

namespace UI.Settings
{
    using Components;

    public class ControlMappingPreview : Control
    {
        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ControlMappingPreview.tscn");

        private IControlMapping controlMapping;
        private Action<IControlMapping> deleteFunc;

        public ControlMappingPreview Config(Node parent, SimInput.IControlMapping _controlMapping, Action<IControlMapping> _deleteFunc)
        {
            controlMapping = _controlMapping;
            deleteFunc = _deleteFunc;
            if (parent != null) parent.AddChild(this);

            return this;
        }

        public override void _Ready()
        {
            GetNode<Label>("Label").Text = CreateControlMappingText();
        }

        private string CreateControlMappingText()
        {
            if (controlMapping == null)
            {
                Utils.LogError("Control mapping is null", this);
                return "Null";
            }
            if (controlMapping is AxisControlMapping am) return $"Joystick - axis {am.Axis}";
            if (controlMapping is MomentaryKeyboardControlMapping mm) return $"Keyboard (momentary) - {OS.GetScancodeString(mm.KeyScancode)}";
            if (controlMapping is ToggleKeyboardControlMapping tm) return $"Keyboard (toggle) - {OS.GetScancodeString(tm.KeyScancode)}";
            if (controlMapping is ThreePosKeyboardControlMapping p3m)
                return $"Keyboard (3 position) - " +
                $"{OS.GetScancodeString(p3m.Key1Scancode)}, {OS.GetScancodeString(p3m.Key2Scancode)}, {OS.GetScancodeString(p3m.Key3Scancode)}";

            Utils.LogError($"Unknown control type: {controlMapping.GetType().FullName}", this);
            return "Unknown mapping type";
        }

        private void _on_Edit_pressed()
        {
            GD.Print("Sorry this doesn't do anything yet");
        }

        private void _on_Delete_pressed()
        {
            deleteFunc(controlMapping);
        }
    }
}
