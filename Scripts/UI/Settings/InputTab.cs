using Godot;
using System;

namespace UI.Settings
{
    public class InputTab : Control
    {
        public override void _Ready()
        {
            var holder = GetNode<VBoxContainer>("VBoxContainer/AccordionMenu");
            var mappings = SimSettings.Settings.Current.InputMap.Channels;
            for (int i = 0; i < mappings.Count; i++)
            {
                var mapping = mappings[i];
                var name = mapping.Name.Capitalize();
                if (mapping.Description != null && mapping.Description != "") name = $"{name} - {mapping.Description}";
                InputChannelEditor.Scene.Instance<InputChannelEditor>().Config(holder, name, i);
            }
        }

        private void _on_Reset_pressed()
        {
            GetNode<ConfirmationDialog>("CustomConfirmationDialog").PopupCenteredMinsize();
        }

        private void _on_ConfirmationDialog_confirmed()
        {
            SettingsScreen.NewSettings.InputMap = new();
            SettingsScreen.ChangeSettings();
        }
    }
}