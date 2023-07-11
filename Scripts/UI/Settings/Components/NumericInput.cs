using Godot;
using System;

namespace UI.Settings.Components;

public partial class NumericInput : SettingsRow<float>
{

    private SpinBox spinBox;

    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/NumericInput.tscn");

    public NumericInput Config(Node parent, string name, SettingReader<float> read, SettingWriter<float> write, float min = 0, float max = 1, float step = 0.01f, string toolTip = "")
    {
        base.Config(parent, name, read, write, toolTip);

        spinBox = GetNode<SpinBox>("SpinBox");

        spinBox.MinValue = min;
        spinBox.MaxValue = max;
        spinBox.Step = step;

        return this;
    }

    public void _on_SpinBox_value_changed(float value)
    {
        if (SettingsScreen.NewSettings != null) write(SettingsScreen.NewSettings, value);
    }

    public override void OnSettingsChanged()
    {
        spinBox.Value = read(SettingsScreen.NewSettings);
    }
}