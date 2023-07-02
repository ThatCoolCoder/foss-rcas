using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public class InputMap
    {
        // How the input mappings are stored

        // Map of action path to list of mappings
        public Dictionary<string, List<IControlMapping>> Mappings { get; set; } = new();

        public List<IControlMapping> GetMappingsForAction(string actionPath)
        {
            // returns non-copied list of mappings, so changing the value will change the mappings

            if (Mappings.TryGetValue(actionPath, out var mappings)) return mappings;
            else
            {
                var items = new List<IControlMapping>();
                Mappings[actionPath] = items;
                return items;
            }
        }

        public static readonly InputMap DefaultMap = new()
        {
            Mappings =
        {
            // AIRCRAFT
            {"aircraft/throttle", new() {
                new SimInput.AxisControlMapping() { Axis = 0 }}
            },
            {"aircraft/aileron", new() {
                new SimInput.AxisControlMapping() { Axis = 2 }}
            },
            {"aircraft/elevator", new() {
                new SimInput.AxisControlMapping() { Axis = 3 }}
            },
            {"aircraft/rudder", new() {
                new SimInput.AxisControlMapping() { Axis = 1 }}
            },
            {"aircraft/aux1", new() {
                new SimInput.ThreePosKeyboardControlMapping()
                {
                    Key1Scancode = (uint) KeyList.Key5,
                    Key2Scancode = (uint) KeyList.Key6,
                    Key3Scancode = (uint) KeyList.Key7,
                }}
            },
            {"aircraft/aux2", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.F,
                    Momentary = true
                }}
            },
            {"aircraft/aux3", new() {
                new SimInput.ThreePosKeyboardControlMapping()
                {
                    Key1Scancode = (uint) KeyList.Key8,
                    Key2Scancode = (uint) KeyList.Key9,
                    Key3Scancode = (uint) KeyList.Key0,
                }}
            },
            {"aircraft/aux4", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.G,
                    Momentary = false
                }}
            },

            // CAMERA
            {"camera/move_backward_forward", new() },
            {"camera/move_backward", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.S,
                    Momentary = true
                },
            }},
            {"camera/move_forward", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.W,
                    Momentary = true
                },
            }},
            {"camera/move_left_right", new() },
            {"camera/move_left", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.A,
                    Momentary = true
                },
            }},
            {"camera/move_right", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.D,
                    Momentary = true
                },
            }},
            {"camera/move_down_up", new() },
            {"camera/move_down", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.Q,
                    Momentary = true
                },
            }},
            {"camera/move_up", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.E,
                    Momentary = true
                },
            }},
            {"camera/pan_combined", new() },
            {"camera/pan_left", new() },
            {"camera/pan_right", new() },
            {"camera/tilt_combined", new() },
            {"camera/tilt_down", new() },
            {"camera/tilt_up", new() },

            {"camera/reset", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.C | (uint) KeyModifierMask.MaskCmd,
                    Momentary = true
                },
            }},
            {"camera/previous", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.C | (uint) KeyModifierMask.MaskShift,
                    Momentary = true
                },
            }},
            {"camera/next", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.C,
                    Momentary = true
                },
            }},

            // GAMEPLAY
            {"gameplay/launch", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.Space,
                    Momentary = true
                },
            }},
            {"gameplay/reset", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.R,
                    Momentary = true
                },
            }},
            {"gameplay/reload_aircraft", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.R | (uint) KeyModifierMask.MaskCmd,
                    Momentary = true
                },
            }},
            {"gameplay/pause", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    KeyScancode = (uint) KeyList.P,
                    Momentary = true
                },
            }},
        },
        };
    };
}