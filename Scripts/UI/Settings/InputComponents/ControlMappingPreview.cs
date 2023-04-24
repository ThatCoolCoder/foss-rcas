using Godot;
using System;
using System.Collections.Generic;

using SimInput;

namespace UI.Settings.InputComponents
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
            // todo: perhaps there is a better way to do this than a bunch of ifs?
            // Trouble with lambdas is we need a specific type in the lambda but a dict of lamdas would have unspecific type
            if (controlMapping == null)
            {
                Utils.LogError("Control mapping is null", this);
                return "Null";
            }
            if (controlMapping is AxisControlMapping am) return $"Joystick - axis {am.Axis}";
            if (controlMapping is ButtonControlMapping bm) return $"Joystick - button {bm.ButtonIndex}";
            if (controlMapping is MomentaryKeyboardControlMapping mm) return $"Keyboard (momentary) - {OS.GetScancodeString(mm.KeyScancode)}";
            if (controlMapping is ToggleKeyboardControlMapping tm) return $"Keyboard (toggle) - {OS.GetScancodeString(tm.KeyScancode)}";
            if (controlMapping is ThreePosKeyboardControlMapping p3m)
                return $"Keyboard (3 position) - " +
                $"{OS.GetScancodeString(p3m.Key1Scancode)}, {OS.GetScancodeString(p3m.Key2Scancode)}, {OS.GetScancodeString(p3m.Key3Scancode)}";

            Utils.LogError($"Unknown control type: {controlMapping.GetType().FullName}", this);
            return "Unknown mapping type";
        }

        private BaseControlMappingEditor CreateEditor()
        {
            if (controlMapping == null) Utils.LogError("Control mapping is null", this);
            if (controlMapping is AxisControlMapping am) return AxisMappingEditor.Scene.Instance<AxisMappingEditor>().Config(this, am);
            if (controlMapping is ButtonControlMapping bm) return ButtonMappingEditor.Scene.Instance<ButtonMappingEditor>().Config(this, bm);
            if (controlMapping is MomentaryKeyboardControlMapping mkm)
                return MomentaryKeyboardMappingEditor.Scene.Instance<MomentaryKeyboardMappingEditor>().Config(this, mkm);
            if (controlMapping is ToggleKeyboardControlMapping tkm)
                return ToggleKeyboardMappingEditor.Scene.Instance<ToggleKeyboardMappingEditor>().Config(this, tkm);
            if (controlMapping is ThreePosKeyboardControlMapping tpkm)
                return ThreePosKeyboardMappingEditor.Scene.Instance<ThreePosKeyboardMappingEditor>().Config(this, tpkm);
            return null;
        }

        private void _on_Edit_pressed()
        {
            var editor = CreateEditor();
            editor.PopupCentered();
        }

        private void _on_Delete_pressed()
        {
            deleteFunc(controlMapping);
        }
    }
}
