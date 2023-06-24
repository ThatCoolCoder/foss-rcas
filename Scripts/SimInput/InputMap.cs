using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public class InputMap
    {
        // Predefined mappers for InputAction.mapTo
        private static Func<float, float> extraMapperNegativeHalf = x => -(x / 2 + .5f);
        private static Func<float, float> extraMapperPositiveHalf = x => x / 2 + .5f;

        // This defines the channels that are available in the sim, and also the default mappings
        public List<InputActionCategory> ActionCategories { get; set; } = new()
        {
            new("aircraft", "aircraft", new() {
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
                        Key1Scancode = (uint) KeyList.Key5,
                        Key2Scancode = (uint) KeyList.Key6,
                        Key3Scancode = (uint) KeyList.Key7,
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
                        Key1Scancode = (uint) KeyList.Key8,
                        Key2Scancode = (uint) KeyList.Key9,
                        Key3Scancode = (uint) KeyList.Key0,
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
            }),
            new("camera", "camera", new () {
                InputAction.FromNoMapping("move_backward_forward", displayName: "move forward/backward (combined)",
                    description: " (in some camera modes this zooms instead)"),
                InputAction.FromSingleMapping("move_backward",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint) KeyList.S,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move backward", mapTo: new () {
                        {"camera/move_backward_forward", extraMapperNegativeHalf}
                    }),
                InputAction.FromSingleMapping("move_forward",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint)KeyList.W,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move forward", mapTo: new() {
                        {"camera/move_backward_forward", extraMapperPositiveHalf}
                    }),

                InputAction.FromNoMapping("move_left_right", displayName: "move left/right (combined)"),
                InputAction.FromSingleMapping("move_left",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint)KeyList.A,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move left", mapTo: new() {
                        {"camera/move_left_right", extraMapperNegativeHalf}
                    }),
                InputAction.FromSingleMapping("move_right",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint)KeyList.D,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move right", mapTo: new() {
                        {"camera/move_left_right", extraMapperPositiveHalf}
                    }),

                InputAction.FromNoMapping("move_down_up", displayName: "move up/down (combined)"),
                InputAction.FromSingleMapping("move_down",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint)KeyList.Q,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move down", mapTo: new() {
                        {"camera/move_down_up", extraMapperNegativeHalf}
                    }),
                InputAction.FromSingleMapping("move_up",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint)KeyList.E,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "move up", mapTo: new() {
                        {"camera/move_down_up", extraMapperPositiveHalf}
                    }),

                InputAction.FromNoMapping("pan_combined", displayName: "turn left/right (combined)"),
                InputAction.FromNoMapping("pan_left",
                    defaultValue: -1, displayName: "turn left", mapTo: new() {
                        {"camera/pan_combined", extraMapperNegativeHalf}
                    }),
                InputAction.FromNoMapping("pan_right",
                    defaultValue: -1, displayName: "turn right", mapTo: new() {
                        {"camera/pan_combined", extraMapperPositiveHalf}
                    }),

                InputAction.FromNoMapping("tilt_combined", displayName: "turn up/down (combined)"),
                InputAction.FromNoMapping("tilt_up",
                    defaultValue: -1, displayName: "turn up", mapTo: new() {
                        {"camera/tilt_combined", extraMapperNegativeHalf}
                    }),
                InputAction.FromNoMapping("tilt_down",
                    defaultValue: -1, displayName: "turn down", mapTo: new() {
                        {"camera/tilt_combined", extraMapperPositiveHalf}
                    }),
                InputAction.FromSingleMapping("reset",
                    new SimInput.SimpleKeyboardControlMapping()
                    {
                        KeyScancode = (uint)KeyList.C,
                        Momentary = true
                    },
                    defaultValue: -1, displayName: "reset", description: "- resets the camera angle"),
            }),
            new("gameplay", "gameplay", new() {
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
            })
        };
    }
}