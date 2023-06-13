using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public class InputMap
    {
        public Dictionary<string, List<InputAction>> ActionCategories { get; set; } = new()
        {
            // This defines the channels that are available in the sim, in addition to the default mappings
            { "aircraft", new() {
                InputAction.FromSingleMapping("throttle",
                    new SimInput.AxisControlMapping() { Axis = 0 }, defaultValue: -1),
                InputAction.FromSingleMapping("aileron",
                    new SimInput.AxisControlMapping() { Axis = 2 }),
                InputAction.FromSingleMapping("elevator",
                    new SimInput.AxisControlMapping() { Axis = 3 }),
                InputAction.FromSingleMapping("rudder",
                    new SimInput.AxisControlMapping() { Axis = 1 },
                    defaultValue: -1),

                InputAction.FromSingleMapping("aux1",
                    new SimInput.ThreePosKeyboardControlMapping()
                    {
                        Key1Scancode = (uint) KeyList.Key8,
                        Key2Scancode = (uint) KeyList.Key9,
                        Key3Scancode = (uint) KeyList.Key0,
                    },
                    defaultValue: -1,
                    displayName: "aux 1", description: " - used for things like flaps"),
                InputAction.FromSingleMapping("aux2",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.F,
                        Momentary = true
                    },
                    defaultValue: -1,
                    displayName: "aux 2", description: " - used for things like bomb drops"),
                InputAction.FromSingleMapping("aux3",
                    new SimInput.ThreePosKeyboardControlMapping()
                    {
                        Key1Scancode = (uint) KeyList.I,
                        Key2Scancode = (uint) KeyList.O,
                        Key3Scancode = (uint) KeyList.P,
                    },
                    defaultValue: -1,
                    displayName: "aux 3", " - used for things like spoilers/airbrakes"),
                InputAction.FromSingleMapping("aux4",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.G,
                        Momentary = false
                    },
                    defaultValue: -1,
                    displayName: "aux 4", description: " - normally landing gear"),
            }},
            { "camera", new() {
                InputAction.FromNoMapping("move_forward_backward",
                    defaultValue: 0, displayName: "move forward/backward (combined)",
                    description: " (in some camera modes this zooms instead)"),
                InputAction.FromSingleMapping("move_forward",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.W,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move forward"),
                InputAction.FromSingleMapping("move_backward",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.S,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move backward"),

                InputAction.FromNoMapping("move_left_right",
                    defaultValue: 0, displayName: "move left/right (combined)"),
                InputAction.FromSingleMapping("move_left",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.A,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move left"),
                InputAction.FromSingleMapping("move_right",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.D,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move right"),

                InputAction.FromNoMapping("move_up_down",
                    defaultValue: 0, displayName: "move up/down (combined)"),
                InputAction.FromSingleMapping("move_up",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.Q,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move up"),
                InputAction.FromSingleMapping("move_down",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.Z,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move down"),

                InputAction.FromNoMapping("pan_combined",
                    defaultValue: 0, displayName: "turn left/right (combined)"),
                InputAction.FromNoMapping("pan_left",
                    defaultValue: -1, displayName: "turn left"),
                InputAction.FromNoMapping("pan_right",
                    defaultValue: -1, displayName: "turn right"),

                InputAction.FromNoMapping("tilt_combined",
                    defaultValue: 0, displayName: "turn up/down (combined)"),
                InputAction.FromNoMapping("tilt_up",
                    defaultValue: -1, displayName: "turn up"),
                InputAction.FromNoMapping("tilt_down",
                    defaultValue: -1, displayName: "turn down"),
            }},
            { "gameplay", new() {
                InputAction.FromSingleMapping("launch",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.Space,
                        Momentary = true
                    },
                    defaultValue: -1),
                InputAction.FromSingleMapping("reset",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.R,
                        Momentary = true
                    },
                    defaultValue: -1),
                InputAction.FromSingleMapping("pause",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.P,
                        Momentary = true
                    },
                    defaultValue: -1),
            }}
        };
    }
}