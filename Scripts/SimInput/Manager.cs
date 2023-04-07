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
            var channels = SimSettings.Settings.Current?.InputChannels;
            if (channels != null)
            {
                foreach (var channel in channels)
                {
                    foreach (var mapping in channel.Mappings)
                    {
                        // Read value
                        if (mapping.ProcessEvent(_event) is float val) axisValues[channel.Name] = val;

                        // Init defaults
                        else if (!axisValues.ContainsKey(channel.Name)) axisValues[channel.Name] = channel.DefaultValue;
                    }
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