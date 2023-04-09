using Godot;
using System;

namespace Aircraft
{
    public class ControlledMotor : Physics.Forcers.Motor, Control.IControllable
    {
        // Motor controlled by the keyboard

        [Export] public string ThrottleActionName { get; set; }
        [Export] public bool Reversible { get; set; }

        public Control.Hub ControlHub { get; set; }

        public override void _Process(float delta)
        {
            ThrustProportion = ControlHub.ChannelValues[ThrottleActionName];
            if (!Reversible) ThrustProportion = ThrustProportion / 2 + 0.5f;
            base._Process(delta);
        }
    }
}
