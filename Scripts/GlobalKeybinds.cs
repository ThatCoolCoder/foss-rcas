using Godot;
using System;

public partial class GlobalKeybinds : Node
{
    // Class handling global (as in throughout the entire game) keybinds.

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        // This one is nice to have global so we can trigger it before loading in the physics stuff (which may only check it on _ready)
        if (SimInput.Manager.IsActionJustPressed("global/toggle_physics_debug")) Physics.Forcers.AbstractSpatialFluidForcer.SetDebugModeActive(!Physics.Forcers.AbstractSpatialFluidForcer.DebugModeActive);

        if (SimInput.Manager.IsActionJustPressed("global/take_screenshot")) SaveScreenshot();

        // This one is completely a dev thing so there is no reason to move it to siminput
        if (Input.IsActionJustPressed("complete_reset")) GetTree().ChangeSceneToFile("res://Scenes/UI/StartScreen.tscn");
    }

    private void SaveScreenshot()
    {
        var data = GetViewport().GetTexture().GetImage();

        var dir = OS.GetSystemDir(OS.SystemDir.Pictures);
        var formattedTime = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var path = $"{dir}/FRC-Screenshot-{formattedTime}.png";
        data.SavePng(path);
    }
}