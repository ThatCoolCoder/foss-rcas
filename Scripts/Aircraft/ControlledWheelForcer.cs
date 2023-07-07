using Godot;
using System;

namespace Aircraft
{
    public class ControlledWheelForcer : Physics.Forcers.WheelForcer, Control.IControllable
    {
        [Export] public string DriveActionName { get; set; }
        [Export] public string BrakeActionName { get; set; }
        [Export] public bool ReversibleDrive { get; set; } // whether can drive forward and reverse
        [Export] public bool Backwards { get; set; }
        public Control.IHub ControlHub { get; set; }

        public override void _Process(float delta)
        {
            var powerProportion = DriveActionName == null ? 0 : ControlHub.ChannelValues[DriveActionName];
            if (Backwards) powerProportion *= -1;
            if (!ReversibleDrive) powerProportion = powerProportion / 2 + 0.5f;

            WheelDriveFactor = powerProportion;
            var brakeProportion = BrakeActionName == null ? 0 : ControlHub.ChannelValues[BrakeActionName];
            WheelBrakeFactor = Utils.MapNumber(brakeProportion, -1, 1, 0, 1);

            base._Process(delta);
        }
    }
}