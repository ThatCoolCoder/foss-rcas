using Godot;
using System;

namespace UI.Apps
{
    public partial class BrushlessMotorDebug : AbstractBasicNodeDebug<Physics.Motors.BrushlessMotor>
    {
        protected override string GroupName { get; set; } = "BrushlessMotor";

        protected override string GenerateText(Physics.Motors.BrushlessMotor motor)
        {
            var propeller = motor.GetNode<Physics.Forcers.Propeller>(motor.PropellerPath);
            var battery = motor.GetNode<Aircraft.Battery>(motor.BatteryPath);
            return $"Motor name: {motor.Name}\n" +
                $"KV: {motor.KV:F0}\n" +
                $"Peak torque: {motor.PeakTorque:F3}\n" +
                $"Clockwise: {motor.Clockwise}\n" +
                $"Throttle: {motor.ThrustProportion * 100:F0}%\n" +
                $"Unloaded rpm: {motor.KV * battery.CurrentVoltage:F0}\n" +
                $"Rpm: {propeller.Rpm:F0}\n" +
                $"Torque: {motor.LastTorque:F3}Nm\n" +
                $"Current: {motor.LastCurrent:F2}A\n";
        }
    }
}