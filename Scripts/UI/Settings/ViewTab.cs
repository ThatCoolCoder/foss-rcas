using Godot;
using System;

namespace UI.Settings;

using Components;
public partial class ViewTab : Control
{

    [Export] public Control Holder { get; set; }

    public override void _Ready()
    {
        NumericInput.Scene.Instantiate<NumericInput>().Config(
            Holder,
            "Field of view (degrees)",
            s => s.View.Fov,
            (s, v) => s.View.Fov = v,
            min: 10, max: 130, step: 1);

        BooleanInput.Scene.Instantiate<BooleanInput>().Config(
            Holder,
            "Zoom enabled",
            s => s.View.GroundCameraZoom.Enabled,
            (s, v) => s.View.GroundCameraZoom.Enabled = v,
            toolTip: "Whether the camera zooms in as the aircraft flies further away");

        NumericInput.Scene.Instantiate<NumericInput>().Config(
            Holder,
            "Zoom start distance",
            s => s.View.GroundCameraZoom.StartDist,
            (s, v) => s.View.GroundCameraZoom.StartDist = v,
            min: 1, max: 1000, step: 1, toolTip: "Distance at which the camera starts zooming in");

        NumericInput.Scene.Instantiate<NumericInput>().Config(
            Holder,
            "Zoom factor",
            s => s.View.GroundCameraZoom.Factor,
            (s, v) => s.View.GroundCameraZoom.Factor = v,
            min: 0, max: 2, step: 0.01f, toolTip: "Controls how much the camera zooms into the aircraft.\nA value of 1 results in the aircraft remaining a constant size regardless of distance.\nValues above 1 are not recommended.");


        NumericInput.Scene.Instantiate<NumericInput>().Config(
            Holder,
            "Minimum FOV",
            s => s.View.GroundCameraZoom.MinFov,
            (s, v) => s.View.GroundCameraZoom.MinFov = v,
            min: 1, max: 130, step: 1f, toolTip: "FOV at which the camera stops zooming");

        BooleanInput.Scene.Instantiate<BooleanInput>().Config(
            Holder,
            "Adjust zoom for aircraft size",
            s => s.View.AdjustZoomForAircraftSize,
            (s, v) => s.View.AdjustZoomForAircraftSize = v,
            toolTip: "Makes the camera zoom in earlier when flying small aircraft and later for large aircraft.\n1 m is the base size; an aircraft this wingspan will start zooming at the regular distance");
    }

    public void _on_Reset_pressed()
    {
        SettingsScreen.NewSettings.View = new();
        SettingsScreen.ChangeSettings();
    }
}