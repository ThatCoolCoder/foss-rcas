using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public class InputMap
    {
        public List<AxisMapping> AxisMappings = new()
        {
            new("aileron", 0),
            new("elevator", 1, multiplier: -1),
            new("rudder", 2),
            new("throttle", 3, multiplier: -1, deadzoneEnd: .025f),
        };
    }
}