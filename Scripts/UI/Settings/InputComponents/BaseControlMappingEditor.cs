using Godot;
using System;

namespace UI.Settings.InputComponents
{
    using Components;

    public class BaseControlMappingEditor : WindowDialog
    {
        // It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
        // The performance should still be fine and this makes it SO much easier to code.

        private void _on_Close_pressed()
        {
            SettingsScreen.ChangeSettings();
            Hide();
        }

        protected VBoxContainer GetMainItemHolder()
        {
            return GetNode<VBoxContainer>("MarginContainer/VBoxContainer");
        }
    }
}
