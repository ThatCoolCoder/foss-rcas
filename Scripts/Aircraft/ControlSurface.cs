using Godot;
using System;

namespace Aircraft
{
    public class ControlSurface : Spatial, IControllable
    {
        // Object that rotates around its X-axis

        [Export] public float MaxDeflectionDegrees { get; set; } = 30;
        [Export] public bool Reversed { get; set; }
        [Export] public string ChannelName { get; set; } = "";

        public ControlHub ControlHub { get; set; }

        public override void _Process(float delta)
        {
            var rawValue = ControlHub.ChannelValues[ChannelName];
            rawValue *= Mathf.Deg2Rad(MaxDeflectionDegrees);
            if (Reversed) rawValue *= -1;
            Rotation = Rotation.WithX(rawValue);
        }

    }
}