using Godot;
using System;

public class GlobalKeybinds : Node
{
    // Class handling global (as in throughout the entire game) keybinds. At this point it just handles stuff that siminput can't do yet.

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float delta)
    {
        // This one is nice to have global so we can trigger it before loading in the physics stuff (which may only check it on _ready)
        if (Input.IsActionJustPressed("toggle_physics_debug")) Physics.Forcers.AbstractSpatialFluidForcer.SetDebugModeActive(!Physics.Forcers.AbstractSpatialFluidForcer.DebugModeActive);

        if (Input.IsActionJustPressed("complete_reset")) GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
        if (Input.IsActionJustPressed("screenshot")) SaveScreenshot();
    }

    private void SaveScreenshot()
    {
        var data = GetViewport().GetTexture().GetData();
        data.FlipY();

        var dir = OS.GetSystemDir(OS.SystemDir.Pictures);
        var formattedTime = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var path = $"{dir}/FRC-Screenshot-{formattedTime}.png";
        data.SavePng(path);
    }
}