using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SimInput
{
    public class Manager : Node
    {
        // Godot's default input manager is woefully lacking in that it has no option to combine left/right movement of a stick into a single input.
        // So this is a simpler input manager that should be better for this use
        // Possible todo: see if there is any way to get rid of this by writing a custom module for the regular input manager.

        private static Manager Instance;

        private Dictionary<int, AxisMapping> axisLookup = new();

        // Map of axis name to axis value
        private Dictionary<string, float> axisValues = new();

        public override void _EnterTree()
        {
            Instance = this;

            var axisMappings = SimSettings.Settings.Current?.InputMap?.AxisMappings;
            if (axisMappings != null)
            {
                axisLookup = axisMappings.ToDictionary(x => x.Axis, x => x);
                axisValues = axisMappings.ToDictionary(x => x.Name, x => 0f);
            }
        }

        public override void _ExitTree()
        {
            Instance = null;
        }

        public override void _Input(InputEvent _event)
        {
            if (_event is InputEventJoypadMotion motionEvent)
            {
                try
                {
                    var value = motionEvent.AxisValue;
                    var mapping = axisLookup[motionEvent.Axis];
                    axisValues[mapping.Name] = mapping.Apply(value);
                }
                catch (KeyNotFoundException)
                {
                    // do nothing - this axis is clearly unimportant
                }
            }
        }
        public static float GetAxisValue(string actionName)
        {
            try
            {
                return Instance.axisValues[actionName];
            }
            catch (KeyNotFoundException)
            {
                Utils.LogError($"Unknown action: {actionName}");
                return 0;
            }
        }
    }
}