using System;
using Godot;
using ContentManagement;

namespace UI.FlightSettings.AircraftConfig;

public partial class StringInput : AbstractInput
{
    [Export] private LineEdit LineEdit { get; set; }
    public AircraftConfigProperty.String Property { get; set; }
    protected override AircraftConfigProperty AircraftConfigProperty { get { return Property; } }

    public override void Reset()
    {
        LineEdit.Text = Property.DefaultValue;
    }

    public override dynamic GetValue()
    {
        return LineEdit.Text;
    }
}