using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public partial class InputMap
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
                new SimInput.AxisControlMapping() { Axis = (uint)JoyAxis.LeftY }}
            },
            {"aircraft/aileron", new() {
                new SimInput.AxisControlMapping() { Axis = (uint)JoyAxis.RightX }}
            },
            {"aircraft/elevator", new() {
                new SimInput.AxisControlMapping() { Axis = (uint)JoyAxis.RightY }}
            },
            {"aircraft/rudder", new() {
                new SimInput.AxisControlMapping() { Axis = (uint)JoyAxis.LeftX }}
            },
            {"aircraft/aux1", new() {
                new SimInput.ThreePosKeyboardControlMapping()
                {
                    Key1Scancode = (uint) Key.Key5,
                    Key2Scancode = (uint) Key.Key6,
                    Key3Scancode = (uint) Key.Key7,
                }}
            },
            {"aircraft/aux2", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.F,
                    Momentary = true
                }}
            },
            {"aircraft/aux3", new() {
                new SimInput.ThreePosKeyboardControlMapping()
                {
                    Key1Scancode = (uint) Key.Key8,
                    Key2Scancode = (uint) Key.Key9,
                    Key3Scancode = (uint) Key.Key0,
                }}
            },
            {"aircraft/aux4", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.G,
                    Momentary = false
                }}
            },

            // CAMERA
            {"camera/move_backward_forward", new() },
            {"camera/move_backward", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.S,
                    Momentary = true
                },
            }},
            {"camera/move_forward", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.W,
                    Momentary = true
                },
            }},
            {"camera/move_left_right", new() },
            {"camera/move_left", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.A,
                    Momentary = true
                },
            }},
            {"camera/move_right", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.D,
                    Momentary = true
                },
            }},
            {"camera/move_down_up", new() },
            {"camera/move_down", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.Q,
                    Momentary = true
                },
            }},
            {"camera/move_up", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.E,
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
                    Key = Key.C | (Key) KeyModifierMask.MaskCmdOrCtrl,
                    Momentary = true
                },
            }},
            {"camera/previous", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.C | (Key) KeyModifierMask.MaskShift,
                    Momentary = true
                },
            }},
            {"camera/next", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.C,
                    Momentary = true
                },
            }},

            // GAMEPLAY
            {"gameplay/launch", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.Space,
                    Momentary = true
                },
            }},
            {"gameplay/reset", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.R,
                    Momentary = true
                },
            }}, // convtodo: it doesn't like keycodes with ctrl
            {"gameplay/reload_aircraft", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.R | (Key) KeyModifierMask.MaskCmdOrCtrl,
                    Momentary = true
                },
            }},
            {"gameplay/pause", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.P,
                    Momentary = true
                },
            }},
            {"gameplay/more_slow_motion", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.Comma,
                    Momentary = true
                },
            }},
            {"gameplay/less_slow_motion", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.Period,
                    Momentary = true
                },
            }},
            {"gameplay/reset_slow_motion", new() {
                new SimInput.SimpleKeyboardControlMapping()
                {
                    Key = Key.Slash,
                    Momentary = true
                },
            }},
        },
        };
    };
}