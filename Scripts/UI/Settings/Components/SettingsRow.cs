using Godot;
using System;

namespace UI.Settings.Components
{
    public abstract class SettingsRow<T> : HSplitContainer
    {
        // Base class for settings row

        // suggested usage: DerivedScene.Instance<DerivedClass>().Config(parentNode, "Awesome setting" s => s.a, (s, v) => s.a = v );
        // You can also provide null instead of a parent and manually add it to the scene

        protected private string name { get; private set; }
        protected SettingReader<T> read { get; private set; }
        protected SettingWriter<T> write { get; private set; }

        protected SettingsRow<T> Config(Node parent, string _name, SettingReader<T> _read, SettingWriter<T> _write, string toolTip = "")
        {
            // We can't use the constructor because godot takes over that, so we need a custom init method

            if (parent != null) parent.AddChild(this);

            read = _read;
            write = _write;
            name = _name;

            GetNode<Label>("Label").Text = name;

            HintTooltip = toolTip;

            SettingsScreen.OnSettingsChanged += OnSettingsChanged;

            return this;
        }

        public abstract void OnSettingsChanged();

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }
    }
}