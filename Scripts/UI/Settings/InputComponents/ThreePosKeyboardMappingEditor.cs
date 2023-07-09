using Godot;
using System;

namespace UI.Settings.InputComponents
{
    using Components;

    public partial class ThreePosKeyboardMappingEditor : BaseControlMappingEditor
    {
        // It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
        // The performance should still be fine and this makes it SO much easier to code.

        private SimInput.ThreePosKeyboardControlMapping controlMapping;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ThreePosKeyboardMappingEditor.tscn");

        public ThreePosKeyboardMappingEditor Config(Node parent, SimInput.ThreePosKeyboardControlMapping _controlMapping)
        {
            controlMapping = _controlMapping;
            if (parent != null) parent.AddChild(this);

            return this;
        }

        public override void _Ready()
        {
            var holder = GetMainItemHolder();

            holder.GetNode<KeyInput>("KeyInput1").Config(null, "Position 1",
                s => controlMapping.Key1Scancode,
                (s, v) => controlMapping.Key1Scancode = v)
                .OnSettingsChanged();
            holder.GetNode<KeyInput>("KeyInput2").Config(null, "Position 2",
                s => controlMapping.Key2Scancode,
                (s, v) => controlMapping.Key2Scancode = v)
                .OnSettingsChanged();
            holder.GetNode<KeyInput>("KeyInput3").Config(null, "Position 3",
                s => controlMapping.Key3Scancode,
                (s, v) => controlMapping.Key3Scancode = v)
                .OnSettingsChanged();
        }
    }
}
