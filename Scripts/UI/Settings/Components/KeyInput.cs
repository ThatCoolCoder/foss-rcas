using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Settings.Components
{
    public class KeyInput : BaseInputInput<uint, uint?>
    {
        private uint? lastPressedKey;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/KeyInput.tscn");

        public new KeyInput Config(Node parent, string name, SettingReader<uint> read, SettingWriter<uint> write, string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);

            return this;
        }

        protected override void OnInputWhenOpen(InputEvent _event)
        {
            if (_event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
            {
                lastPressedKey = keyEvent.GetScancodeWithModifiers();

                UpdatePopup();
            }
        }

        protected override uint? GetCandidateValue()
        {
            return lastPressedKey;
        }

        protected override string GetPopupText()
        {
            var candidate = GetCandidateValue();

            if (candidate is uint notNull) return $"{OS.GetScancodeString(notNull)} selected";
            else return "Press a key or key combination to select it...";
        }

        protected override string GetCurrentValueText()
        {
            return OS.GetScancodeString(read(SettingsScreen.NewSettings));
        }

        protected override void ClearCandidateValue()
        {
            lastPressedKey = null;
        }

        protected override bool ShouldShowSelectAgainButton()
        {
            return false;
        }
    }
}