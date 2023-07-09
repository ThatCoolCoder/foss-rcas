using Godot;
using System;

namespace UI.Settings
{
    using Components;
    public partial class ViewTab : Control
    {

        public override void _Ready()
        {
            var holder = GetNode<VBoxContainer>("MaxSizeContainer/VBoxContainer/MainList");

            NumericInput.Scene.Instantiate<NumericInput>().Config(
                holder,
                "Field of view (degrees)",
                s => s.GroundCameraZoom.BaseFov,
                (s, v) => s.GroundCameraZoom.BaseFov = v,
                min: 10, max: 130, step: 1);

            BooleanInput.Scene.Instantiate<BooleanInput>().Config(
                holder,
                "Zoom enabled",
                s => s.GroundCameraZoom.Enabled,
                (s, v) => s.GroundCameraZoom.Enabled = v,
                toolTip: "Whether the camera zooms in as the aircraft flies further away");


            NumericInput.Scene.Instantiate<NumericInput>().Config(
                holder,
                "Zoom start distance",
                s => s.GroundCameraZoom.StartDist,
                (s, v) => s.GroundCameraZoom.StartDist = v,
                min: 1, max: 1000, step: 1, toolTip: "Distance at which the camera starts zooming in");

            NumericInput.Scene.Instantiate<NumericInput>().Config(
                holder,
                "Zoom factor",
                s => s.GroundCameraZoom.Factor,
                (s, v) => s.GroundCameraZoom.Factor = v,
                min: 0, max: 2, step: 0.01f, toolTip: "Controls how much the camera zooms into the aircraft.\nA value of 1 results in the aircraft remaining a constant size regardless of distance.\nValues above 1 are not recommended.");
        }

        public void _on_Reset_pressed()
        {
            SettingsScreen.NewSettings.GroundCameraZoom = new();
            SettingsScreen.ChangeSettings();
        }
    }
}