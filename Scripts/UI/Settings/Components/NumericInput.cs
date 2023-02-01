using Godot;
using System;



namespace UI.Settings.Components
{
    public class NumericInput : SettingsRow<float>
    {

        private SpinBox spinBox;

        public static new PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/NumericInput.tscn");

        public NumericInput Config(string name, SettingReader<float> read, SettingWriter<float> write, float min = 0, float max = 1, bool rounded = false)
        {
            base.Config(name, read, write);

            spinBox = GetNode<SpinBox>("SpinBox");

            spinBox.MinValue = min;
            spinBox.MaxValue = max;
            spinBox.Rounded = rounded;

            return this;
        }

        public void _on_SpinBox_value_changed(float value)
        {
            write(value);
        }
    }
}