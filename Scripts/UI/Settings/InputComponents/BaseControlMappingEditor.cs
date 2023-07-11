using Godot;
using System;

namespace UI.Settings.InputComponents;

using Components;

public partial class BaseControlMappingEditor : Window
{
    // It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
    // The performance should still be fine and this makes it SO much easier to code.

    public Action DeleteFunc;

    private void _on_Save_pressed()
    {
        SettingsScreen.ChangeSettings();
        Hide();
    }

    private void _on_Delete_pressed()
    {
        Hide();
        DeleteFunc();
    }

    protected VBoxContainer GetMainItemHolder()
    {
        return GetNode<VBoxContainer>("MarginContainer/VBoxContainer");
    }
}
