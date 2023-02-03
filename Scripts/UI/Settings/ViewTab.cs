using Godot;
using System;

namespace UI.Settings
{
    using Components;
    public class ViewTab : VBoxContainer
    {

        public override void _Ready()
        {
            var holder = GetNode<VBoxContainer>("MainList");

            NumericInput.Scene.Instance<NumericInput>().Config(
                holder,
                "Field of view (degrees)",
                s => s.GroundCameraZoomSettings.BaseFov,
                (s, v) => s.GroundCameraZoomSettings.BaseFov = v,
                min: 10, max: 130, step: 1);

            BooleanInput.Scene.Instance<BooleanInput>().Config(
                holder,
                "Zoom enabled",
                s => s.GroundCameraZoomSettings.Enabled,
                (s, v) => s.GroundCameraZoomSettings.Enabled = v,
                toolTip: "Whether the camera zooms in as the aircraft flies further away");


            NumericInput.Scene.Instance<NumericInput>().Config(
                holder,
                "Zoom start distance",
                s => s.GroundCameraZoomSettings.StartDist,
                (s, v) => s.GroundCameraZoomSettings.StartDist = v,
                min: 1, max: 1000, step: 1, toolTip: "Distance at which the camera starts zooming in");

            NumericInput.Scene.Instance<NumericInput>().Config(
                holder,
                "Zoom factor",
                s => s.GroundCameraZoomSettings.Factor,
                (s, v) => s.GroundCameraZoomSettings.Factor = v,
                min: 0, max: 2, step: 0.01f, toolTip: "Controls how much the camera zooms into the aircraft.\nA value of 1 results in the aircraft remaining a constant size regardless of distance.\nValues above 1 are not recommended.");
        }

        public void _on_Reset_pressed()
        {
            SettingsScreen.NewSettings.GroundCameraZoomSettings = new();
            SettingsScreen.ChangeSettings();
        }
    }
}