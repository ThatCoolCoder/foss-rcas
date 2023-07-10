using Godot;
using System;

namespace UI.Settings.InputComponents
{
    using Components;

    public partial class AxisMappingEditor : BaseControlMappingEditor
    {
        // It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
        // The performance should still be fine and this makes it SO much easier to code.

        private SimInput.AxisControlMapping controlMapping;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/AxisMappingEditor.tscn");

        public AxisMappingEditor Config(Node parent, SimInput.AxisControlMapping _controlMapping)
        {
            controlMapping = _controlMapping;
            if (parent != null) parent.AddChild(this);

            return this;
        }

        public override void _Ready()
        {
            Func<float, string> percentageDisplayFunc = x => Utils.RoundToPlaces(x * 100, 0).ToString() + "%";

            var holder = GetMainItemHolder();
            holder.GetNode<BooleanInput>("InvertedInput").Config(null, "Inverted",
                s => controlMapping.Inverted,
                (s, v) => controlMapping.Inverted = v)
                .OnSettingsChanged();

            holder.GetNode<JoystickAxisInput>("JoystickAxisInput").Config(null, "Selected axis",
                s => (JoyAxis)controlMapping.Axis,
                (s, v) => controlMapping.Axis = (uint)v)
                .OnSettingsChanged();

            holder.GetNode<NumericSliderInput>("Sensitivity").Config(null, "Sensitivity",
                s => controlMapping.Multiplier,
                (s, v) => controlMapping.Multiplier = v,
                min: 0, max: 2, step: 0.01f, _customDisplayFunc: percentageDisplayFunc)
                .OnSettingsChanged();

            holder.GetNode<NumericSliderInput>("DeadzoneRest").Config(null, "Deadzone (rest)",
                s => controlMapping.DeadzoneRest,
                (s, v) => controlMapping.DeadzoneRest = v,
                min: 0, max: 2, step: 0.01f, _customDisplayFunc: percentageDisplayFunc)
                .OnSettingsChanged();

            holder.GetNode<NumericSliderInput>("DeadzoneEnd").Config(null, "Deadzone (end)",
                s => controlMapping.DeadzoneEnd,
                (s, v) => controlMapping.DeadzoneEnd = v,
                min: 0, max: 2, step: 0.01f, _customDisplayFunc: percentageDisplayFunc)
                .OnSettingsChanged();
        }
    }
}
