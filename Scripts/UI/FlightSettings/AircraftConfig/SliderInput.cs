using System;
using Godot;
using ContentManagement;

namespace UI.FlightSettings.AircraftConfig;

public partial class SliderInput : AbstractInput
{
    [Export] private HSlider hSlider { get; set; }
    [Export] private Label displayLabel { get; set; }
    public AircraftConfigProperty.Slider Property { get; set; }
    protected override AircraftConfigProperty AircraftConfigProperty { get { return Property; } }

    public override void _Ready()
    {
        base._Ready();
        hSlider.MinValue = Property.Min;
        hSlider.MaxValue = Property.Max;
        hSlider.Step = Property.Step;
    }

    public override void Reset()
    {
        hSlider.Value = Property.DefaultValue;
        _on_h_slider_value_changed((float)hSlider.Value);
    }

    public override dynamic GetValue()
    {
        return (float)hSlider.Value;
    }

    private void _on_h_slider_value_changed(float value)
    {
        displayLabel.Text = Decimal.Round(new Decimal(value), Property.DecimalsDisplayed).ToString();
    }
}