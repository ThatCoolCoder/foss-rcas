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
        // Todo: add support for customisation of controls, and for more channels.
        // Possible todo: see if there is any way to get rid of this by writing a custom module for the regular input manager.

        private static Manager instance;

        public static List<AxisMapping> AxisMappings = new()
        {
            new("aileron", 0),
            new("elevator", 1, multiplier: -1),
            new("rudder", 2),
            new("throttle", 3, multiplier: -1),
        };

        private static Dictionary<int, AxisMapping> axisLookup = new();

        // Map of axis name to axis value
        private static Dictionary<string, float> axisValues = new();

        static Manager()
        {
            axisLookup = AxisMappings.ToDictionary(x => x.Axis, x => x);
            axisValues = AxisMappings.ToDictionary(x => x.Name, x => 0f);
        }

        public override void _EnterTree()
        {
            instance = this;
        }

        public override void _ExitTree()
        {
            instance = null;
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
                return axisValues[actionName];
            }
            catch (KeyNotFoundException)
            {
                // GD.PrintErr($"Unknown action: {actionName}");
                return 0;
            }
        }
    }
}