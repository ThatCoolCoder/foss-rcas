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
        public float Offset { get; set; } = 0;
        public float SpeedUp { get; set; } = 0; // % of movement per second when value is increasing. EG with a value of 0.5 it will take two seconds to go from 0 to 1. Value of 0 means instant.
        public float SpeedDown { get; set; } = 0;
        public ChannelMixMode Mode { get; set; } = ChannelMixMode.Add;

        private float currentValue = 0;

        public float Apply(float inputValue, float channelValue, float delta)
        {
            // Apply this mix to the existing channel value.
            // Note that there is some state stored within the ChannelMix so don't go creating new ones too often

            var targetValue = inputValue + Offset;

            // this should match the expo calculation used by open/edge tx
            targetValue = Expo * Mathf.Pow(targetValue, 3) + (1 - Expo) * targetValue;
            targetValue *= Weight;

            var speed = currentValue > targetValue ? SpeedDown : SpeedUp;
            currentValue = speed == 0 ? targetValue : Utils.ConvergeValue(currentValue, targetValue, speed * delta);

            if (Mode == ChannelMixMode.Add) channelValue += currentValue;
            else if (Mode == ChannelMixMode.Multiply) channelValue *= currentValue;
            else if (Mode == ChannelMixMode.Replace) channelValue = currentValue;

            return channelValue;
        }
    }
}