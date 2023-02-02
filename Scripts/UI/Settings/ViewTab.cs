using Godot;
using System;

namespace UI.Settings
{
    public class ViewTab : VBoxContainer
    {
        // todo: switch to reflection the auto-settings?

        public override void _Ready()
        {
            AddChild(NumericInput.Scene.Instance<NumericInput>().Config(
                "Field of View"
                s => s.GroundCameraZoomSettings.BaseFov,
                (s, v) => s.GroundCameraZoomSettings.BaseFov = v,
                10, 130, true));

            // with reflection looks like:
            // AddChild(NumericInput.Scene.Instance<NumericInput>().Config(
            //     "Field of View"
            //     "GroundCameraZoomSettings.BaseFov",
            //     10, 130, true));
        }

        public void _on_Reset_pressed()
        {
            SettingsScreen.NewSettings.GroundCameraZoomSettings = new();
            SettingsScreen.OnSettingsChanged.Invoke();
        }
    }
}