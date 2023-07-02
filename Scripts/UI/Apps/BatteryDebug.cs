using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Apps
{
    public class BatteryDebug : Control
    {
        // Basic implementation just visually displaying values of the aircraft channels.
        // todo: in future, create a version with a little tx that has sticks that move around, and visual indications for switches

        private Label label;
        private SpinBox batteryIndexSelector;
        private int RescanFrequency = 240;
        private List<NodePath> foundBatteries = new();
        private NodePath currentBatteryPath;

        public override void _Ready()
        {
            label = GetNode<Label>("MarginContainer/VBoxContainer/Output");
            batteryIndexSelector = GetNode<SpinBox>("MarginContainer/VBoxContainer/HBoxContainer/SpinBox");

            ScanForBatteries();
        }

        public override void _Process(float delta)
        {
            if (Engine.GetFramesDrawn() % RescanFrequency == 0) ScanForBatteries();

            var battery = GetNode<Aircraft.Battery>(currentBatteryPath);
            if (battery == null) label.Text = "Battery not found";
            else
            {
                var percentageRemaining = battery.RemainingCapacity / battery.MaxCapacity * 100;
                label.Text = $"Cell count: {battery.CellCount}\n" +
                    $"Total capacity: {battery.MaxCapacity:F2}Ah\n" +
                    $"Remaining: {battery.RemainingCapacity:F2}Ah ({percentageRemaining:F1}%)\n" +
                    $"Cell voltage range: {battery.FlatCellVoltage:F2}V - {battery.ChargedCellVoltage:F2}V\n" +
                    $"Current cell voltage: {battery.CurrentCellVoltage:F2}\n" +
                    $"Current: {battery.CurrentDrawn:F2}A\n";
            }
        }

        private void ScanForBatteries()
        {
            foundBatteries = GetTree().GetNodesInGroup("battery").ToList<Node>().Select(x => x.GetPath()).ToList();
            if (foundBatteries.Count > 0)
            {
                batteryIndexSelector.MinValue = 0;
                batteryIndexSelector.MaxValue = foundBatteries.Count - 1;
                batteryIndexSelector.Editable = true;
            }
            else
            {
                batteryIndexSelector.Editable = false;
            }
        }

        private void _on_SpinBox_changed()
        {
            currentBatteryPath = foundBatteries[(int)batteryIndexSelector.Value];
        }
    }
}