using Godot;
using System;
using System.Collections.Generic;

namespace Aircraft
{
    public class ChannelMix
    {
        public string InputChannelName { get; set; }
        public string OutputChannelName { get; set; }
        public float Weight { get; set; } = 1;
        public float Expo { get; set; } = 0;

        public float Apply(float value)
        {
            // Apply weight and expo to a value

            // this should match the expo calculation used by open/edge tx

            value = Expo * (value * value * value) + (1 - Expo) * value;
            value *= Weight;

            return value;
        }
    }
}