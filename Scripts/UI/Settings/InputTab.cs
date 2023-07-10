using Godot;
using System;

namespace UI.Settings
{
    using InputComponents;

    public partial class InputTab : Control
    {
        [Export] public ConfirmationDialog ConfirmationDialog { get; set; }
        [Export] public Control Holder { get; set; }

        public override void _Ready()
        {
            foreach (var category in SimInput.AvailableInputActions.Categories)
            {
                var name = category.Name.Capitalize();
                ActionCategoryEditor.Scene.Instantiate<ActionCategoryEditor>().Config(Holder, name, category);
            }
        }

        private void _on_Reset_pressed()
        {
            ConfirmationDialog.PopupCenteredClamped();
        }

        private void _on_ConfirmationDialog_confirmed()
        {
            SettingsScreen.NewSettings.InputMap = new();
            SettingsScreen.ChangeSettings();
        }
    }
}