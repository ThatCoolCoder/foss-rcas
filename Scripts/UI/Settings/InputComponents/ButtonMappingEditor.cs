using Godot;
using System;

namespace UI.Settings.InputComponents
{
    using Components;

    public partial class ButtonMappingEditor : BaseControlMappingEditor
    {
        // It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
        // The performance should still be fine and this makes it SO much easier to code.

        private SimInput.ButtonControlMapping controlMapping;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ButtonMappingEditor.tscn");

        public ButtonMappingEditor Config(Node parent, SimInput.ButtonControlMapping _controlMapping)
        {
            controlMapping = _controlMapping;
            if (parent != null) parent.AddChild(this);

            return this;
        }

        public override void _Ready()
        {
            var holder = GetMainItemHolder();

            holder.GetNode<JoystickButtonInput>("JoystickButtonInput").Config(null, "Button",
                s => controlMapping.ButtonIndex,
                (s, v) => controlMapping.ButtonIndex = v)
                .OnSettingsChanged();

            holder.GetNode<BooleanInput>("MomentaryInput").Config(null, "Momentary",
                s => controlMapping.Momentary,
                (s, v) => controlMapping.Momentary = v,
                toolTip: "Whether the input is momentary or toggles")
                .OnSettingsChanged();

            holder.GetNode<BooleanInput>("InvertedInput").Config(null, "Inverted",
                s => controlMapping.Inverted,
                (s, v) => controlMapping.Inverted = v,
                toolTip: "Only applies if it is a momentary input ")
                .OnSettingsChanged();
        }
    }
}
