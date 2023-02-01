using Godot;
using System;



namespace UI.Settings
{
    public class NumericInput : SettingsRow
    {

        NumericInput(SettingReader<int> _get, SettingWriter<int> _set)
        {
            get = _get;
            set = _set;
        }

        public override void _Ready()
        {
            base._Ready();
        }
    }
}