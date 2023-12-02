using System;
using Godot;
using ContentManagement;

namespace UI.FlightSettings.AircraftConfig;

public partial class EnumInput : AbstractInput
{
    [Export] private OptionButton optionButton;
    public AircraftConfigProperty.Enum Property { get; set; }
    protected override AircraftConfigProperty AircraftConfigProperty { get { return Property; } }

    public override void _Ready()
    {
        optionButton.Clear();
        foreach (var value in Property.PossibleValues) optionButton.AddItem(value);
        base._Ready();
    }

    public override void Reset()
    {
        optionButton.Selected = Property.PossibleValues.IndexOf(Property.DefaultValue);
    }

    public override dynamic GetValue()
    {
        return Property.PossibleValues[optionButton.Selected];
    }
}