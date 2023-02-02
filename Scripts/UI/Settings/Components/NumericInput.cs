using Godot;
using System;

namespace UI.Settings.Components
{
    public class NumericInput : SettingsRow<float>
    {

        private SpinBox spinBox;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/NumericInput.tscn");

        public NumericInput Config(Node parent, string name, SettingReader<float> read, SettingWriter<float> write, float min = 0, float max = 1, bool rounded = false, string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);

            spinBox = GetNode<SpinBox>("SpinBox");

            spinBox.MinValue = min;
            spinBox.MaxValue = max;
            spinBox.Rounded = rounded;

            return this;
        }

        public void _on_SpinBox_value_changed(float value)
        {
            if (SettingsScreen.NewSettings != null) write(SettingsScreen.NewSettings, value);
        }

        protected override void OnSettingsChanged()
        {
            spinBox.Value = read(SettingsScreen.NewSettings);
        }
    }
}