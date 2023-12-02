using System;
using Godot;
using ContentManagement;

namespace UI.FlightSettings.AircraftConfig;

public partial class BooleanInput : AbstractInput
{
    public AircraftConfigProperty.Boolean Property { get; set; }
    protected override AircraftConfigProperty AircraftConfigProperty { get { return Property; } }
    [Export] private CheckBox checkBox { get; set; }

    public override void Reset()
    {
        checkBox.ButtonPressed = Property.DefaultValue;
    }

    public override dynamic GetValue()
    {
        return checkBox.ButtonPressed ? 1 : 0;
    }
}