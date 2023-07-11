using Godot;
using System;
using System.Collections.Generic;

namespace SimInput;

public static class AvailableInputActions

{
    // Source of truth for what input actions and categories exist in the sim

    // Predefined mappers for InputAction.mapTo
    private static Func<float, float> extraMapperNegativeHalf = x => -(x / 2 + .5f);
    private static Func<float, float> extraMapperPositiveHalf = x => x / 2 + .5f;

    public static List<InputActionCategory> Categories { get; set; } = new()
    {
        new("aircraft", "aircraft", new() {
            new InputAction("throttle", defaultValue: -1),
            new InputAction("aileron"),
            new InputAction("elevator"),
            new InputAction("rudder"),

            new InputAction("aux1", "aux 1", " - used for things like flaps",
            defaultValue: -1),
            new InputAction("aux2", "aux 2", " - used for things like bomb drops",
                defaultValue:-1),
            new InputAction("aux3", "aux 3", " - used for things like spoilers/airbrakes",
                defaultValue:-1),
            new InputAction("aux4", "aux 4", " - normally landing gear",
                defaultValue: -1),
        }),
        new("camera", "camera", new () {
            new InputAction("move_backward_forward", "move forward/backward (combined)",
                " (in some camera modes this zooms instead)"),
            new InputAction("move_backward", "move backward", defaultValue: -1,
                mapTo: new () {
                    {"camera/move_backward_forward", extraMapperNegativeHalf}
                }),
            new InputAction("move_forward", "move forward", defaultValue: -1,
                mapTo: new() {
                    {"camera/move_backward_forward", extraMapperPositiveHalf}
                }),

            new InputAction("move_left_right", "move left/right (combined)"),
            new InputAction("move_left", "move left", defaultValue: -1,
                mapTo: new() {
                    {"camera/move_left_right", extraMapperNegativeHalf}
                }),
            new InputAction("move_right", "move right", defaultValue: -1,
                mapTo: new() {
                    {"camera/move_left_right", extraMapperPositiveHalf}
                }),

            new InputAction("move_down_up", "move up/down (combined)"),
            new InputAction("move_down", "move down", defaultValue: -1,
                mapTo: new() {
                    {"camera/move_down_up", extraMapperNegativeHalf}
                }),
            new InputAction("move_up", "move up", defaultValue: -1,
                mapTo: new() {
                    {"camera/move_down_up", extraMapperPositiveHalf}
                }),

            new InputAction("pan_combined", "turn left/right (combined)"),
            new InputAction("pan_left", "turn left", defaultValue: -1,
                mapTo: new() {
                    {"camera/pan_combined", extraMapperNegativeHalf}
                }),
            new InputAction("pan_right", "turn right", defaultValue: -1,
                mapTo: new() {
                    {"camera/pan_combined", extraMapperPositiveHalf}
                }),

            new InputAction("tilt_combined", "turn up/down (combined)"),
            new InputAction("tilt_up", "turn up", defaultValue: -1,
                mapTo: new() {
                    {"camera/tilt_combined", extraMapperNegativeHalf}
                }),
            new InputAction("tilt_down", "turn down", defaultValue: -1,
                mapTo: new() {
                    {"camera/tilt_combined", extraMapperPositiveHalf}
                }),

            new InputAction("reset", "reset", " - resets the current camera",
                defaultValue: -1),

            new InputAction("previous", "previous",  " - switch to the previous camera",
                defaultValue: -1),
            new InputAction("next", "next",  " - switch to the next camera",
                defaultValue: -1),
        }),
        new("gameplay", "gameplay", new() {
            new InputAction("launch",
                defaultValue: -1),
            new InputAction("reset",
                defaultValue: -1),
            new InputAction("reload_aircraft",
                defaultValue: -1),
            new InputAction("pause",
                defaultValue: -1),
            new InputAction("more_slow_motion",
                defaultValue: -1),
            new InputAction("less_slow_motion",
                defaultValue: -1),
            new InputAction("reset_slow_motion",
                defaultValue: -1),
        })
    };
}