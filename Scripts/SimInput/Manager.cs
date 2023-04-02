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

        // Map of axis name to axis value
        private static Dictionary<string, float> axisValues = new();

        public override void _EnterTree()
        {
            Instance = this;
        }

        public override void _ExitTree()
        {
            Instance = null;
        }

        public override void _Input(InputEvent _event)
        {
            var controlMappings = SimSettings.Settings.Current?.ControlMappings;
            if (controlMappings != null)
            {
                foreach (var m in controlMappings)
                {
                    if (m.ProcessEvent(_event) is float val) axisValues[m.ChannelName] = val;
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
                // Utils.LogError($"Unknown action: {actionName}"); // todo: We don't put values in until there is an inputevent, so there will be lots of this error at first if it's not commented out
                return 0;
            }
        }
    }
}