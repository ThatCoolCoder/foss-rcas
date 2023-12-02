using System;
using Godot;
using ContentManagement;

namespace UI.FlightSettings.AircraftConfig;

public partial class SpinBoxInput : AbstractInput
{
    [Export] private SpinBox spinBox { get; set; }
    public AircraftConfigProperty.SpinBox Property { get; set; }
    protected override AircraftConfigProperty AircraftConfigProperty { get { return Property; } }

    public override void _Ready()
    {
        spinBox.Step = Property.Step;
        spinBox.CustomArrowStep = Property.ArrowStep < 0 ? Property.Step : Property.ArrowStep;
        spinBox.MinValue = Property.Min;
        spinBox.MaxValue = Property.Max;
        base._Ready();
    }

    public override void Reset()
    {
        GD.Print(spinBox.Step);
        spinBox.Value = Property.DefaultValue;
    }

    public override dynamic GetValue()
    {
        return (float)spinBox.Value;
    }
}
