using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Aircraft.Control
{
    public partial class FourChannelHub : Node3D, IHub
    {
        // super-simple four channel hub supporting no mixing, designed for beginners

        [Export] public float Expo { get; set; } = 0.3f;
        public Dictionary<string, float> ChannelValues { get; set; } = new();

        private static readonly List<(string Name, bool HasExpo)> channels = new()
        {
            ("throttle", false), ("aileron", true), ("elevator", true), ("rudder", true)
        };

        public override void _Process(double delta)
        {
            ChannelValues = channels.ToDictionary(x => x.Name, x =>
            {
                var baseValue = SimInput.Manager.GetActionValue("aircraft/" + x.Name);
                return x.HasExpo ? ChannelMix.CalculateExpo(baseValue, Expo) : baseValue;
            });
        }
    }
}