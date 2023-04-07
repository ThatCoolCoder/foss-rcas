using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public class InputMap
    {
        public List<Channel> Channels { get; set; } = new()
        {
            Channel.FromSingleMapping("throttle",
                new SimInput.AxisControlMapping() { Axis = 0 }),
            Channel.FromSingleMapping("aileron",
                new SimInput.AxisControlMapping() { Axis = 2 }),
            Channel.FromSingleMapping("elevator",
                new SimInput.AxisControlMapping() { Axis = 3 }),
            Channel.FromSingleMapping("rudder",
                new SimInput.AxisControlMapping() { Axis = 1 },
                defaultValue: -1),

            Channel.FromSingleMapping("gear",
                new SimInput.ToggleKeyboardControlMapping() { KeyScancode = (uint) KeyList.G },
                defaultValue: -1),
            Channel.FromSingleMapping("aux1",
                new SimInput.ThreePosKeyboardControlMapping()
                {
                    Key1Scancode = (uint) KeyList.A,
                    Key2Scancode = (uint) KeyList.S,
                    Key3Scancode = (uint) KeyList.D,
                },
                defaultValue: -1, description: "normally flaps"),
            Channel.FromSingleMapping("aux2",
                new SimInput.MomentaryKeyboardControlMapping() { KeyScancode = (uint) KeyList.B },
                defaultValue: -1, description: "used for things like bomb drops"),
            Channel.FromSingleMapping("aux3",
                new SimInput.ThreePosKeyboardControlMapping()
                {
                    Key1Scancode = (uint) KeyList.Q,
                    Key2Scancode = (uint) KeyList.W,
                    Key3Scancode = (uint) KeyList.E,
                },
                defaultValue: -1, description: "used for things like spoilers/airbrakes"),
        };
    }
}