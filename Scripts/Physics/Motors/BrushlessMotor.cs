using Godot;
using System;

namespace Physics.Motors
{
    public class BrushlessMotor : Spatial
    {
        [Export] public NodePath BatteryPath { get; set; }
        private Aircraft.Battery battery;
        [Export] public NodePath PropellerPath { get; set; }
        private Forcers.Propeller propeller;
        [Export] public float KV { get; set; } = 1000;
        [Export] public float PeakTorque { get; set; } = .4f; // in newton-metres
        [Export] public float TorqueAdjustment { get; set; } = 1; // Use this to fine-tune the rpm without messing up the regular peak torque adjustment
        [Export] public float CurrentMultiplier { get; set; } = 1; // If you have measurements of the current drawn under load, you can use this to make the calculated current match
        [Export] public bool Clockwise { get; set; } = true;
        public float ThrustProportion { get; set; } = 0;

        public override void _Ready()
        {
            battery = Utils.GetNodeWithWarnings<Aircraft.Battery>(this, BatteryPath, "battery");
            propeller = Utils.GetNodeWithWarnings<Forcers.Propeller>(this, PropellerPath, "propeller");
        }

        public override void _PhysicsProcess(float delta)
        {

            var noLoadRpm = KV * battery.CurrentVoltage * ThrustProportion;
            if (!Clockwise) noLoadRpm = -noLoadRpm;
            
            float torqueProportion = 0;
            if (noLoadRpm != 0) torqueProportion = (noLoadRpm - propeller.Rpm) / noLoadRpm;
            var torque = torqueProportion * PeakTorque * TorqueAdjustment;
            propeller.AddTorque(torque);

            var torqueConstant = 1 / (KV / 60 * Mathf.Tau);
            battery.Discharge(torque/torqueConstant * CurrentMultiplier, delta); // todo: does this recharge the battery when the prop is windmilling? It shouldn't.
        }
    }
}
