using Godot;
using System;

namespace UI.FlightSettings.AircraftConfig;

public abstract partial class AbstractInput : HBoxContainer
{
    // Would be nice to have a generic T : AircraftConfigProperty and store that here instead of derived but c# can't handle that
    [Export] private Label label { get; set; }

    public override void _Ready()
    {
        label.Text = AircraftConfigProperty.DisplayName;
        TooltipText = $"{AircraftConfigProperty.Description} (internal name: {AircraftConfigProperty.Name})";
        Reset();
    }

    protected abstract ContentManagement.AircraftConfigProperty AircraftConfigProperty { get; }
    public abstract void Reset();
    public abstract dynamic GetValue();
}