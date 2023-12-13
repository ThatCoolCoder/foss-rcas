using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Physics.Motors;

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
    [Export] public float LowVoltageCutoffStart { get; set; } = 3.3f;
    [Export] public float LowVoltageCutoffEnd { get; set; } = 2.75f;
    public float ThrustProportion { get; set; } = 0;
    public float LastTorque { get; private set; }
    public float LastCurrent { get; private set; }

    private const float lowVoltageAverageDuration = 1;
    private List<float> lastCellVoltages = new();

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

        // Keep track of average battery voltage
        lastCellVoltages.Add(battery.CurrentCellVoltage);
        var maxVoltageCount = lowVoltageAverageDuration / delta;
        while (lastCellVoltages.Count > maxVoltageCount) lastCellVoltages.RemoveAt(0);
        var averageCellVoltage = lastCellVoltages.Average();

        // Apply low voltage cutoff
        ThrustProportion *= Mathf.Clamp(Utils.MapNumber(averageCellVoltage, LowVoltageCutoffStart, LowVoltageCutoffEnd, 1, 0), 0, 1);

        float torque = 0;
        if (ThrustProportion > 0.02f)
        {
            noLoadRpm *= ThrustProportion;
            var torqueProportion = (noLoadRpm - propeller.Rpm) / noLoadRpm;
            torque = torqueProportion * PeakTorque * TorqueAdjustment * Mathf.Sign(noLoadRpm);

            torque = Mathf.Clamp(torque, 0, PeakTorque);

            var torqueConstant = 1 / (KV / 60 * Mathf.Tau);
            var current = torque / torqueConstant * CurrentMultiplier;
            current = Mathf.Max(current, 0);
            battery.Discharge(current, (float)delta);
            propeller.ApplyTorque(torque);

            LastTorque = torque;
            LastCurrent = current;
        }
        else
        {
            LastTorque = 0;
            LastCurrent = 0;
        }
        propeller.AddBrakingTorque(PeakTorque * TorqueAdjustment * 0.05f);

        if (torqueRigidBody != null && !torqueRigidBody.Freeze)
        {
            torqueRigidBody.ApplyTorque(GlobalTransform.Basis.Z * torque);
        }
    }
}


