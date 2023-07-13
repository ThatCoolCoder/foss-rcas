using Godot;
using System;

namespace UI.Settings.Components;

public partial class NumericSliderInput : SettingsRow<float>
{
    // Input for numeric values using a slider. Step value controls actual value,
    // displayedDecimalPlaces controls decimal places shown when using default formatter, 
    // customDisplayFunc allows having a completely custom format

    private Godot.Range slider;
    private Label valueLabel;
    private string formatString;
    private Func<float, string> customDisplayFunc { get; set; } = null;

    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/NumericSliderInput.tscn");

    public NumericSliderInput Config(Node parent, string name, SettingReader<float> read, SettingWriter<float> write,
        float min = 0, float max = 1, float step = 0.01f, string _formatString = "0.##",
        Func<float, string> _customDisplayFunc = null,
        string toolTip = "")
    {
        base.Config(parent, name, read, write, toolTip);

        valueLabel = GetNode<Label>("HSplitContainer/ValueLabel");
        slider = GetNode<Godot.Range>("HSplitContainer/HSlider");

        slider.MinValue = min;
        slider.MaxValue = max;
        slider.Step = step;
        customDisplayFunc = _customDisplayFunc;
        formatString = _formatString;

        _on_HSlider_value_changed((float)slider.Value);

        return this;
    }

    private void _on_HSlider_drag_ended(bool _unused)
    {
        if (SettingsScreen.NewSettings != null) write(SettingsScreen.NewSettings, (float)slider.Value);
    }

    private void _on_HSlider_value_changed(float value)
    {
        valueLabel.Text = customDisplayFunc == null ? value.ToString(formatString) : customDisplayFunc(value);
    }

    public override void OnSettingsChanged()
    {
        slider.Value = read(SettingsScreen.NewSettings);
    }
}