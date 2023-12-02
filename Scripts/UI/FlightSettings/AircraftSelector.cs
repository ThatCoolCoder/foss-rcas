using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using ContentManagement;

namespace UI.FlightSettings;

public partial class AircraftSelector : AbstractContentSelector<ContentManagement.Aircraft>
{
    [Export] private Control NoConfigItems { get; set; }
    private Dictionary<string, AircraftConfig.AbstractInput> configInputs { get; set; } = new();
    [Export] private GridContainer configInputHolder { get; set; }
    [Export] private PackedScene stringInputScene { get; set; }
    [Export] private PackedScene booleanInputScene { get; set; }
    [Export] private PackedScene enumInputScene { get; set; }
    [Export] private PackedScene spinBoxInputScene { get; set; }
    [Export] private PackedScene sliderInputScene { get; set; }

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

    protected override void OnItemSelected()
    {
        // Delete old inputs
        foreach (var item in configInputs.Values)
        {
            configInputHolder.RemoveChild(item);
        }

        configInputs = new();

        NoConfigItems.Visible = SelectedItem.ConfigProperties.Count() == 0;

        // Add inputs for all the values
        foreach (var property in SelectedItem.ConfigProperties)
        {
            AircraftConfig.AbstractInput input = null; // why can't we have covariance on classes in c#?

            if (property is AircraftConfigProperty.String stringProp)
            {
                var stringInput = stringInputScene.Instantiate<AircraftConfig.StringInput>();
                stringInput.Property = stringProp;
                input = (AircraftConfig.AbstractInput)stringInput;
            }
            else if (property is AircraftConfigProperty.Boolean boolProp)
            {
                var booleanInput = booleanInputScene.Instantiate<AircraftConfig.BooleanInput>();
                booleanInput.Property = boolProp;
                input = (AircraftConfig.AbstractInput)booleanInput;
            }
            else if (property is AircraftConfigProperty.Enum enumProp)
            {
                var enumInput = enumInputScene.Instantiate<AircraftConfig.EnumInput>();
                enumInput.Property = enumProp;
                input = (AircraftConfig.AbstractInput)enumInput;
            }
            else if (property is AircraftConfigProperty.Slider sliderProp)
            {
                var sliderInput = sliderInputScene.Instantiate<AircraftConfig.SliderInput>();
                sliderInput.Property = sliderProp;
                input = (AircraftConfig.AbstractInput)sliderInput;
            }
            else if (property is AircraftConfigProperty.SpinBox spinBoxProp)
            {
                var spinBoxInput = spinBoxInputScene.Instantiate<AircraftConfig.SpinBoxInput>();
                spinBoxInput.Property = spinBoxProp;
                input = (AircraftConfig.AbstractInput)spinBoxInput;
            }

            if (input != null)
            {
                configInputs[property.Name] = input;
                configInputHolder.AddChild(input);
            }
        }
    }

    private void _on_reset_pressed()
    {
        foreach (var input in configInputs.Values)
        {
            input.Reset();
        }
    }

    public Dictionary<string, dynamic> GetAircraftConfig()
    {
        return configInputs.ToDictionary(x => x.Key, x => x.Value.GetValue());
    }
}