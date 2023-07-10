using Godot;
using System;

namespace Physics.Motors
{
    public partial class BrushlessMotor : Node3D
    {
        [Export] public NodePath BatteryPath { get; set; }
        private Aircraft.Battery battery;
        [Export] public NodePath PropellerPath { get; set; }
        private Forcers.Propeller propeller;
        [Export] public NodePath TorqueRigidBodyPath { get; set; }
        private RigidBody3D torqueRigidBody;
        [Export] public float KV { get; set; } = 1000;
        [Export] public float PeakTorque { get; set; } = .4f; // in newton-metres
        [Export] public float TorqueAdjustment { get; set; } = 1; // Use this to fine-tune the rpm without messing up the regular peak torque adjustment
        [Export] public float CurrentMultiplier { get; set; } = 1; // If you have measurements of the current drawn under load, you can use this to make the calculated current match
        [Export] public bool Clockwise { get; set; } = true;
        public float ThrustProportion { get; set; } = 0;
        public float LastTorque { get; private set; }
        public float LastCurrent { get; private set; }

        public override void _Ready()
        {
            battery = Utils.GetNodeWithWarnings<Aircraft.Battery>(this, BatteryPath, "battery");
            propeller = Utils.GetNodeWithWarnings<Forcers.Propeller>(this, PropellerPath, "propeller");
            torqueRigidBody = Utils.GetNodeWithWarnings<RigidBody3D>(this, TorqueRigidBodyPath, "torque rigid body");
            AddToGroup("BrushlessMotor");
        }

        public override void _PhysicsProcess(double delta)
        {

            var noLoadRpm = KV * battery.CurrentVoltage;
            if (!Clockwise) noLoadRpm = -noLoadRpm;

            float torque = 0;
            if (ThrustProportion != 0)
            {
                noLoadRpm *= ThrustProportion;
                var torqueProportion = (noLoadRpm - propeller.Rpm) / noLoadRpm;
                torque = torqueProportion * PeakTorque * TorqueAdjustment * Mathf.Sign(noLoadRpm);

                torque = Mathf.Clamp(torque, -PeakTorque, PeakTorque);

                var torqueConstant = 1 / (KV / 60 * Mathf.Tau);
                var current = torque / torqueConstant * CurrentMultiplier;
                current = Mathf.Max(current, 0);
                battery.Discharge(current, (float)delta);
                propeller.ApplyTorque(torque);

                LastCurrent = current;
            }
            else
            {
                torque = PeakTorque * TorqueAdjustment * 0.05f;
                propeller.AddBrakingTorque(torque);

                LastTorque = 0;
                LastCurrent = 0;
            }

            if (torqueRigidBody != null)
            {
                torqueRigidBody.ApplyTorque(GlobalTransform.Basis.Z * torque);
            }
        }
    }
}
