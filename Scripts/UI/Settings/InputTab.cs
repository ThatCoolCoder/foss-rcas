using Godot;
using System;

namespace UI.Settings
{
    using InputComponents;

    public partial class InputTab : Control
    {
        public override void _Ready()
        {
            var holder = GetNode<VBoxContainer>("VBoxContainer/AccordionMenu");

            foreach (var category in SimInput.AvailableInputActions.Categories)
            {
                var name = category.Name.Capitalize();
                ActionCategoryEditor.Scene.Instantiate<ActionCategoryEditor>().Config(holder, name, category);
            }
        }

        private void _on_Reset_pressed()
        {
            GetNode<ConfirmationDialog>("CustomConfirmationDialog").PopupCenteredClamped();
        }

        private void _on_ConfirmationDialog_confirmed()
        {
            SettingsScreen.NewSettings.InputMap = new();
            SettingsScreen.ChangeSettings();
        }
    }
}