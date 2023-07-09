using Godot;
using System;

namespace UI.Settings.Components
{
    public partial class TextInput : SettingsRow<string>
    {

        private LineEdit lineEdit;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/TextInput.tscn");

        public new TextInput Config(Node parent, string name, SettingReader<string> read, SettingWriter<string> write, string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);

            lineEdit = GetNode<LineEdit>("LineEdit");

            return this;
        }

        public void _on_LineEdit_text_changed(string value)
        {
            if (SettingsScreen.NewSettings != null) write(SettingsScreen.NewSettings, value);
        }

        public override void OnSettingsChanged()
        {
            lineEdit.Text = read(SettingsScreen.NewSettings);
        }
    }
}