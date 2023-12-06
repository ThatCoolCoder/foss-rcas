using Godot;
using System;
using System.Collections.Generic;

namespace SimInput;

public partial class InputMap
{
    // How the input mappings are stored

    // Map of action path to list of mappings
    public Dictionary<string, List<AbstractControlMapping>> Mappings { get; set; } = new();

    public List<AbstractControlMapping> GetMappingsForAction(string actionPath)
    {
        // returns non-copied list of mappings, so changing the value will change the mappings

        if (Mappings.TryGetValue(actionPath, out var mappings)) return mappings;
        else
        {
            var items = new List<AbstractControlMapping>();
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
            new SimInput.KeyboardControlMapping()
            {
                KeyScancode = (uint) Key.Key5,
                Key2Scancode = (uint) Key.Key6,
                Key3Scancode = (uint) Key.Key7,

                KeyboardMappingType = SimInput.KeyboardControlMapping.MappingTypeEnum.ThreePosition
            }}
        },
        {"aircraft/aux2", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.F,
                KeyboardMappingType = SimInput.KeyboardControlMapping.MappingTypeEnum.Toggle
            }}
        },
        {"aircraft/aux3", new() {
            new SimInput.KeyboardControlMapping()
            {
                KeyScancode = (uint) Key.Key8,
                Key2Scancode = (uint) Key.Key9,
                Key3Scancode = (uint) Key.Key0,
                KeyboardMappingType = SimInput.KeyboardControlMapping.MappingTypeEnum.ThreePosition
            }}
        },
        {"aircraft/aux4", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.G,
                KeyboardMappingType = SimInput.KeyboardControlMapping.MappingTypeEnum.Toggle
            }}
        },

        // CAMERA
        {"camera/move_backward_forward", new() },
        {"camera/move_backward", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.S
            },
        }},
        {"camera/move_forward", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.W
            },
        }},
        {"camera/move_left_right", new() },
        {"camera/move_left", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.A
            },
        }},
        {"camera/move_right", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.D
            },
        }},
        {"camera/move_down_up", new() },
        {"camera/move_down", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.Q,
            },
        }},
        {"camera/move_up", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.E,
            },
        }},
        {"camera/pan_combined", new() },
        {"camera/pan_left", new() },
        {"camera/pan_right", new() },
        {"camera/tilt_combined", new() },
        {"camera/tilt_down", new() },
        {"camera/tilt_up", new() },

        {"camera/reset", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.C | (Key) KeyModifierMask.MaskCmdOrCtrl,
            },
        }},
        {"camera/previous", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.C | (Key) KeyModifierMask.MaskShift,
            },
        }},
        {"camera/next", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.C,
            },
        }},

        // GAMEPLAY
        {"gameplay/launch", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.Space,
            },
        }},
        {"gameplay/reset", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.R,
            },
        }},
        {"gameplay/reload_aircraft", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.R | (Key) KeyModifierMask.MaskCmdOrCtrl,
            },
        }},
        {"gameplay/pause", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.P,
            },
        }},
        {"gameplay/more_slow_motion", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.Comma,
            },
        }},
        {"gameplay/less_slow_motion", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.Period,
            },
        }},
        {"gameplay/reset_slow_motion", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.Slash,
            },
        }},
        {"gameplay/toggle_map", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.M,
            },
        }},

        // GENERAL

        {"general/take_screenshot", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.F1,
            },
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.P | (Key) KeyModifierMask.MaskCmdOrCtrl,
            },
        }},
        {"general/toggle_physics_debug", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.D | (Key) KeyModifierMask.MaskCmdOrCtrl,
            },
        }},
        {"general/toggle_ui", new() {
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.F2,
            },
            new SimInput.KeyboardControlMapping()
            {
                Key = Key.U | (Key) KeyModifierMask.MaskCmdOrCtrl,
            },
        }},
    },
    };
};