using Godot;
using System;

namespace UI.Apps
{
    public class BatteryDebug : AbstractBasicNodeDebug<Aircraft.Battery>
    {
        protected override string GroupName { get; set; } = "Battery";

        protected override string GenerateText(Aircraft.Battery battery)
        {
            var percentageRemaining = battery.RemainingCapacity / battery.MaxCapacity * 100;
            return $"Cell count: {battery.CellCount}\n" +
                $"Total capacity: {battery.MaxCapacity:F2}Ah\n" +
                $"Remaining: {battery.RemainingCapacity:F2}Ah ({percentageRemaining:F1}%)\n" +
                $"Cell voltage range: {battery.FlatCellVoltage:F2}V - {battery.ChargedCellVoltage:F2}V\n" +
                $"Current cell voltage: {battery.CurrentCellVoltage:F2}\n" +
                $"Current: {battery.CurrentDrawn:F2}A\n";
        }
    }
}