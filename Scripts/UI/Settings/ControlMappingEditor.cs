using Godot;
using System;

namespace UI.Settings
{
    using Components;

    public class ControlMappingEditor : Control
    {
        // It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
        // The performance should still be fine and this makes it SO much easier to code.

        private SimInput.AxisControlMapping controlMapping;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/ControlMappingEditor.tscn");

        public ControlMappingEditor Config(Node parent, SimInput.AxisControlMapping _controlMapping)
        {
            controlMapping = _controlMapping;
            if (parent != null) parent.AddChild(this);

            return this;
        }

        public override void _Ready()
        {
            GetNode<BooleanInput>("InvertedInput").Config(null, "Inverted",
                s => controlMapping.Inverted,
                (s, v) => controlMapping.Inverted = v)
                .OnSettingsChanged();

            GetNode<JoystickAxisInput>("JoystickAxisInput").Config(null, "Selected axis",
                s => controlMapping.Axis,
                (s, v) => controlMapping.Axis = v)
                .OnSettingsChanged();

            GetNode<NumericSliderInput>("Sensitivity").Config(null, "Sensitivity",
                s => controlMapping.Multiplier,
                (s, v) => controlMapping.Multiplier = v,
                min: 0, max: 2, step: 0.01f)
                .OnSettingsChanged();

            GetNode<NumericSliderInput>("DeadzoneRest").Config(null, "Deadzone (rest)",
                s => controlMapping.DeadzoneRest,
                (s, v) => controlMapping.DeadzoneRest = v,
                min: 0, max: 2, step: 0.01f)
                .OnSettingsChanged();

            GetNode<NumericSliderInput>("DeadzoneEnd").Config(null, "Deadzone (end)",
                s => controlMapping.DeadzoneEnd,
                (s, v) => controlMapping.DeadzoneEnd = v,
                min: 0, max: 2, step: 0.01f)
                .OnSettingsChanged();
        }
    }
}
