using Godot;
using System;

namespace UI.Settings.Components
{
    public abstract class SettingsRow<T> : HSplitContainer
    {
        // Base class for settings row

        // suggested usage: AddChild(DerivedScene.Instance<DerivedClass>().Config(a, b, c));

        protected private string name { get; private set; }
        protected SettingReader<T> read { get; private set; }
        protected SettingWriter<T> write { get; private set; }

        protected SettingsRow<T> Config(Node parent, string _name, SettingReader<T> _read, SettingWriter<T> _write, string toolTip = "")
        {
            // We can't use the constructor because godot takes over that, so we need a custom init method

            parent.AddChild(this);

            read = _read;
            write = _write;
            name = _name;

            GetNode<Label>("Label").Text = name;

            HintTooltip = toolTip;

            SettingsScreen.OnSettingsChanged += OnSettingsChanged;

            return this;
        }

        protected abstract void OnSettingsChanged();

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }
    }
}