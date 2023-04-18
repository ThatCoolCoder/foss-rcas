using Godot;
using System;

namespace Aircraft
{
    public class BrushlessMotor : Spatial, Control.IControllable
    {
        // Todo: this assumes the 
        [Export] public NodePath BatteryPath { get; set; }
        private Battery battery;
        [Export] public NodePath PropellerPath { get; set; }
        private Physics.Forcers.Propeller propeller;
        [Export] public float KV { get; set; } = 1000;
        [Export] public string ThrottleActionName { get; set; }
        [Export] public bool Reversible { get; set; }

        public Control.Hub ControlHub { get; set; }

        public override void _Ready()
        {
            battery = Utils.GetNodeWithWarnings<Battery>(this, BatteryPath, "battery");
            propeller = Utils.GetNodeWithWarnings<Physics.Forcers.Propeller>(this, PropellerPath, "propeller");
        }

        public override void _Process(float delta)
        {
            var thrustProportion = ControlHub.ChannelValues[ThrottleActionName];
            if (!Reversible) thrustProportion = thrustProportion / 2 + 0.5f;
            var noLoadRpm = KV * battery.CurrentVoltage * thrustProportion;
            propeller.rpm = noLoadRpm * 0.8f; // multiplying by .8 is a decent approximation for loaded rpm, if the motor is propped well

            base._Process(delta);
        }
    }
}
