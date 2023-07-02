using Godot;
using System;

namespace UI.Settings.Components
{
    public class NumericSliderInput : SettingsRow<float>
    {
        // Input for numeric values using a slider. Step value controls actual value,
        // displayedDecimalPlaces controls decimal places shown when using default formatter, 
        // customDisplayFunc allows having a completely custom format

        private Range slider;
        private Label valueLabel;
        private int displayedDecimalPlaces;
        private Func<float, string> customDisplayFunc { get; set; } = null;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/NumericSliderInput.tscn");

        public NumericSliderInput Config(Node parent, string name, SettingReader<float> read, SettingWriter<float> write,
            float min = 0, float max = 1, float step = 0.01f, int _displayedDecimalPlaces = 0,
            Func<float, string> _customDisplayFunc = null,
            string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);

            valueLabel = GetNode<Label>("HSplitContainer/ValueLabel");
            slider = GetNode<Range>("HSplitContainer/HSlider");

            slider.MinValue = min;
            slider.MaxValue = max;
            slider.Step = step;
            customDisplayFunc = _customDisplayFunc;
            displayedDecimalPlaces = _displayedDecimalPlaces;

            _on_HSlider_value_changed((float)slider.Value);

            return this;
        }

        private void _on_HSlider_drag_ended(bool _unused)
        {
            if (SettingsScreen.NewSettings != null) write(SettingsScreen.NewSettings, (float)slider.Value);
        }

        private void _on_HSlider_value_changed(float value)
        {
            valueLabel.Text = customDisplayFunc == null ? value.ToString("F" + displayedDecimalPlaces) : customDisplayFunc(value);
        }

        public override void OnSettingsChanged()
        {
            slider.Value = read(SettingsScreen.NewSettings);
        }
    }
}