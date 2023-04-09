using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Settings.Components
{
    public abstract class BaseInputInput<T, TNullable> : SettingsRow<T>
    {
        // Base input for inputs (eg key presses)

        private Label currentValueLabel;
        private Label popupText;
        private WindowDialog dialog;

        private Button selectAgainButton;
        private Button okButton;

        public override void _Ready()
        {
            currentValueLabel = GetNode<Label>("HBoxContainer/CurrentValue");
            popupText = GetNode<Label>("WindowDialog/MarginContainer/VBoxContainer/PopupText");
            dialog = GetNode<WindowDialog>("WindowDialog");
            selectAgainButton = GetNode<Button>("WindowDialog/MarginContainer/VBoxContainer/HBoxContainer/SelectAnotherInput");
            okButton = GetNode<Button>("WindowDialog/MarginContainer/VBoxContainer/HBoxContainer/Ok");
        }

        public override void OnSettingsChanged()
        {
            currentValueLabel.Text = GetCurrentValueText();
        }

        private void _on_ChangeInput_pressed()
        {
            dialog.PopupCentered();
            ClearCandidateValue();
            UpdatePopup();
        }

        private void _on_Cancel_pressed()
        {
            dialog.Hide();
        }

        private void _on_SelectAnotherInput_pressed()
        {
            ClearCandidateValue();
            UpdatePopup();
        }

        protected void UpdatePopup()
        {
            var candidate = GetCandidateValue();

            popupText.Text = GetPopupText();

            selectAgainButton.Visible = candidate != null && ShouldShowSelectAgainButton();
            okButton.Visible = candidate != null;
        }

        private void _on_Ok_pressed()
        {
            dialog.Hide();

            var value = GetCandidateValue();
            if (value != null && value is T maybeNotNull)
            {
                write(SettingsScreen.NewSettings, maybeNotNull);
                OnSettingsChanged(); // update view
            }
        }

        protected abstract string GetCurrentValueText(); // text to show when not popped up
        protected abstract string GetPopupText(); // text to show in the pop up. EG "select an axis" or "axis 5 selected"
        protected abstract TNullable GetCandidateValue();
        protected abstract void ClearCandidateValue(); // text to show when not popped up


        public override void _Input(InputEvent _event)
        {
            if (dialog?.Visible ?? false) OnInputWhenOpen(_event);
        }

        protected virtual void OnInputWhenOpen(InputEvent _event)
        {
            // feel free to override. It's just like _Input but it only fires when the popup is open
        }

        protected virtual bool ShouldShowSelectAgainButton()
        {
            // Some inputs may just want to automatically select again when the input is pressed, so this button should be suppressed
            return true;
        }
    }
}