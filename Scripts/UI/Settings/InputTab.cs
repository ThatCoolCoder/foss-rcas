using Godot;
using System;

namespace UI.Settings
{
    using InputComponents;

    public class InputTab : Control
    {
        public override void _Ready()
        {
            var holder = GetNode<VBoxContainer>("VBoxContainer/AccordionMenu");

            var categories = SimSettings.Settings.Current.InputMap.ActionCategories;

            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];
                var name = category.Name.Capitalize();
                ActionCategoryEditor.Scene.Instance<ActionCategoryEditor>().Config(holder, name, i);
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