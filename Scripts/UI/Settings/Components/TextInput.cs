using Godot;
using System;

namespace UI.Settings.Components
{
    public class TextInput : SettingsRow<string>
    {

        private LineEdit lineEdit;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/TextInput.tscn");

        public TextInput Config(string name, SettingReader<string> read, SettingWriter<string> write)
        {
            base.Config(name, read, write);

            lineEdit = GetNode<LineEdit>("LineEdit");

            return this;
        }

        public void _on_LineEdit_value_changed(string value)
        {
            write(value);
        }

        protected override void OnSettingsChanged()
        {
            lineEdit.Value = read();
        }
    }
}