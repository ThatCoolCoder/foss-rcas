using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Settings.Components
{
    public partial class JoystickButtonInput : BaseInputInput<JoyButton, JoyButton?>
    {
        private JoyButton? lastPressedButton;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/JoystickButtonInput.tscn");

        public new JoystickButtonInput Config(Node parent, string name, SettingReader<JoyButton> read, SettingWriter<JoyButton> write, string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);

            return this;
        }

        protected override void OnInputWhenOpen(InputEvent _event)
        {
            if (_event is InputEventJoypadButton buttonEvent)
            {
                lastPressedButton = buttonEvent.ButtonIndex;
                UpdatePopup();
            }
        }

        protected override JoyButton? GetCandidateValue()
        {
            // Get the axis that is currently the most moved (and therefore the current candidate)
            // returns null if no axis has been moved enough

            return lastPressedButton;
        }

        protected override string GetPopupText()
        {
            var candidate = GetCandidateValue();

            if (candidate is JoyButton notNull) return $"Button {notNull} selected";
            else return "Press a button on your controller to select it";
        }

        protected override string GetCurrentValueText()
        {
            return $"Button {read(SettingsScreen.NewSettings)}";
        }

        protected override void ClearCandidateValue()
        {
            lastPressedButton = null;
        }

        protected override bool ShouldShowSelectAgainButton()
        {
            return false;
        }
    }
}