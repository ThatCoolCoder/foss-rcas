using Godot;
using System;
using System.Collections.Generic;

using ContentManagement;

namespace UI
{
    public class AircraftSelector : AbstractContentSelector<Aircraft>
    {
        private static readonly Dictionary<AircraftPowerType, string> PowerTypeNames = new()
        {
            {AircraftPowerType.ElectricPropeller, "Electric propeller"},
            {AircraftPowerType.ElectricDuctedFan, "EDF"},
            {AircraftPowerType.InternalCombustion, "Internal combustion"},
            {AircraftPowerType.Turbine, "Turbine"},
            {AircraftPowerType.Other, "Other"},
        };

        protected override string FormatCustomInfo()
        {
            var needsLauncherText = SelectedItem.NeedsLauncher ? "yes" : "no";
            var formattedPowerType = SelectedItem.CustomPowerType == null
                ? PowerTypeNames[SelectedItem.PowerType]
                : SelectedItem.CustomPowerType;

            return
$@"Wingspan: {SelectedItem.WingSpan * 1000:0}mm
Length: {SelectedItem.Length * 1000:0}mm
Weight: {SelectedItem.Weight:0.000}kg
Power type: {formattedPowerType}
Number of channels: {SelectedItem.ChannelCount}
Hand launched: {needsLauncherText}";
        }
    }
}