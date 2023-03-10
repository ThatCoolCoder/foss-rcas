using Godot;
using System;

namespace UI.Settings.Components
{
    public class BooleanInput : SettingsRow<bool>
    {
        private CheckBox checkBox;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/BooleanInput.tscn");

        public new BooleanInput Config(Node parent, string name, SettingReader<bool> read, SettingWriter<bool> write, string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);

            checkBox = GetNode<CheckBox>("CheckBox");

            return this;
        }

        public void _on_CheckBox_toggled(bool value)
        {
            if (SettingsScreen.NewSettings != null) write(SettingsScreen.NewSettings, value);
        }

        protected override void OnSettingsChanged()
        {
            checkBox.SetPressedNoSignal(read(SettingsScreen.NewSettings));
        }
    }
}