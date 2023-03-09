using Godot;
using System;
using System.Collections.Generic;

namespace Aircraft.Control
{
    public class ChannelMix
    {
        public string InputChannelName { get; set; }
        public string OutputChannelName { get; set; }
        public float Weight { get; set; } = 1;
        public float Expo { get; set; } = 0;
        public ChannelMixMode Mode { get; set; } = ChannelMixMode.Add;

        public float Apply(float inputValue, float previousValue)
        {
            // Apply this mix to the previousValue

            // this should match the expo calculation used by open/edge tx

            inputValue = Expo * Mathf.Pow(inputValue, 3) + (1 - Expo) * inputValue;
            inputValue *= Weight;

            if (Mode == ChannelMixMode.Add) previousValue += inputValue;
            else if (Mode == ChannelMixMode.Multiply) previousValue *= inputValue;
            else if (Mode == ChannelMixMode.Replace) previousValue = inputValue;

            return previousValue;
        }
    }
}