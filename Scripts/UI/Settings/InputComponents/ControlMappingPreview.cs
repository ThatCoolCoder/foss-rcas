using Godot;
using System;
using System.Collections.Generic;

using SimInput;

namespace UI.Settings.InputComponents
{
    using Components;

    public class ControlMappingPreview : Button
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

            // todo: use icons for this instead of words
            if (controlMapping == null)
            {
                Utils.LogError("Control mapping is null", this);
                return "Null";
            }
            if (controlMapping is AxisControlMapping am) return $"Axis {am.Axis}";
            if (controlMapping is ButtonControlMapping bm) return $"But {bm.ButtonIndex}";
            if (controlMapping is SimpleKeyboardControlMapping skm)
            {
                if (skm.Momentary) return $"Key - {OS.GetScancodeString(skm.KeyScancode)}";
                else return $"Key - {OS.GetScancodeString(skm.KeyScancode)}";
            }
            if (controlMapping is ThreePosKeyboardControlMapping tpkm)
                return $"Key - " +
                $"{OS.GetScancodeString(tpkm.Key1Scancode)}, {OS.GetScancodeString(tpkm.Key2Scancode)}, {OS.GetScancodeString(tpkm.Key3Scancode)}";

            Utils.LogError($"Unknown control type: {controlMapping.GetType().FullName}", this);
            return "Unknown mapping type";
        }

        private BaseControlMappingEditor CreateEditor()
        {
            if (controlMapping == null) Utils.LogError("Control mapping is null", this);
            if (controlMapping is AxisControlMapping am) return AxisMappingEditor.Scene.Instance<AxisMappingEditor>().Config(this, am);
            if (controlMapping is ButtonControlMapping bm) return ButtonMappingEditor.Scene.Instance<ButtonMappingEditor>().Config(this, bm);
            if (controlMapping is SimpleKeyboardControlMapping skm)
                return SimpleKeyboardMappingEditor.Scene.Instance<SimpleKeyboardMappingEditor>().Config(this, skm);
            if (controlMapping is ThreePosKeyboardControlMapping tpkm)
                return ThreePosKeyboardMappingEditor.Scene.Instance<ThreePosKeyboardMappingEditor>().Config(this, tpkm);
            return null;
        }

        private void _on_ControlMappingPreview_pressed()
        {
            var editor = CreateEditor();
            editor.PopupCentered();
        }
    }
}
