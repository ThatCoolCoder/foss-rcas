using Godot;
using System;

namespace UI.Settings.Components
{
    public abstract class SettingsRow<T> : HSplitContainer
    {
        // Base class for settings row

        protected private string name { get; private set; }
        protected SettingReader<T> read { get; private set; }
        protected SettingWriter<T> write { get; private set; }

        public static PackedScene Scene = null;

        private bool initialized = false;

        protected SettingsRow<T> Config(string _name, SettingReader<T> _read, SettingWriter<T> _write)
        {
            // We can't use the constructor because godot takes over that, so we need a custom init method

            read = _read;
            write = _write;
            name = _name;

            GetNode<Label>("Label").Text = name;

            return this;
        }
    }
}