using Godot;
using System;

namespace SimInput
{
    public class AxisMapping
    {
        // Mapping of joystick axis to custom input manager axis

        public string Name { get; set; }
        public int Axis { get; set; }
        public float Multiplier { get; set; }
        public float DeadzoneEnd { get; set; }
        public float DeadzoneRest { get; set; }

        public AxisMapping(string name, int axis, float multiplier = 1, float deadzoneEnd = 0, float deadzoneRest = 0)
        {
            Name = name;
            Axis = axis;
            Multiplier = multiplier;
            DeadzoneEnd = deadzoneEnd;
            DeadzoneRest = deadzoneRest;
        }

        public float Apply(float value)
        {
            // Apply the mapping to the raw value
            // Inputs and outputs are in the 0 to 1 range

            value = Mathf.Clamp(value * Multiplier, -1, 1);

            if (value < -1 + DeadzoneEnd) value = -1;
            if (value > 1 - DeadzoneEnd) value = 1;
            if (Mathf.Abs(value) < DeadzoneRest) value = 0;

            return value;
        }
    }
}