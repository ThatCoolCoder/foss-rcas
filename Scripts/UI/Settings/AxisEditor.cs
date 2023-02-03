using Godot;
using System;

namespace UI.Settings
{
    using Components;

    public class AxisEditor : Control
    {
        private int axisMappingIndex;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/AxisEditor.tscn");

        public AxisEditor Config(Node parent, string name, int _axisMappingIndex)
        {
            if (parent != null) parent.AddChild(this);
            axisMappingIndex = _axisMappingIndex;

            GetNode<Label>("Heading").Text = name;

            return this;
        }

        public override void _Ready()
        {
            GetNode<BooleanInput>("InvertedInput").Config(null, "Inverted",
                s => GetAxisMapping(s).Inverted,
                (s, v) => GetAxisMapping(s).Inverted = v);

            GetNode<JoystickAxisInput>("JoystickAxisInput").Config(null, "Selected axis",
                s => GetAxisMapping(s).Axis,
                (s, v) => GetAxisMapping(s).Axis = v);

            GetNode<NumericSliderInput>("Sensitivity").Config(null, "Sensitivity",
                s => GetAxisMapping(s).Multiplier,
                (s, v) => GetAxisMapping(s).Multiplier = v,
                min: 0, max: 2, step: 0.01f);

            GetNode<NumericSliderInput>("Expo").Config(null, "Expo",
                s => GetAxisMapping(s).Expo,
                (s, v) => GetAxisMapping(s).Expo = v,
                min: 0, max: 1, step: 0.01f, toolTip: "Expo makes the controls softer near the center,\nmaking it easier to perform fine corrections without limiting the max throw");

            GetNode<NumericSliderInput>("DeadzoneRest").Config(null, "Deadzone (rest)",
                s => GetAxisMapping(s).DeadzoneRest,
                (s, v) => GetAxisMapping(s).DeadzoneRest = v,
                min: 0, max: 2, step: 0.01f);

            GetNode<NumericSliderInput>("DeadzoneEnd").Config(null, "Deadzone (end)",
                s => GetAxisMapping(s).DeadzoneEnd,
                (s, v) => GetAxisMapping(s).DeadzoneEnd = v,
                min: 0, max: 2, step: 0.01f);
        }

        private SimInput.AxisMapping GetAxisMapping(SimSettings.Settings settings)
        {
            return settings.InputMap.AxisMappings[axisMappingIndex];
        }
    }
}
