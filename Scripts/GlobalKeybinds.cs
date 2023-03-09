using Godot;
using System;

public class GlobalKeybinds : Node
{
    // Class handling global (as in throughout the entire game) keybinds

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float delta)
    {
        // This one is nice to have global so we can trigger it before loading in the physics stuff (which may only check it on _ready)
        if (Input.IsActionJustPressed("toggle_physics_debug")) Physics.Forcers.AbstractSpatialFluidForcer.SetDebugModeActive(!Physics.Forcers.AbstractSpatialFluidForcer.DebugModeActive);

        if (Input.IsActionJustPressed("complete_reset")) GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
        if (Input.IsActionJustPressed("pause")) GetTree().Paused = !GetTree().Paused;
        if (Input.IsActionJustPressed("screenshot")) SaveScreenshot();
    }

    private void SaveScreenshot()
    {
        var data = GetViewport().GetTexture().GetData();
        data.FlipY();

        var dir = OS.GetSystemDir(OS.SystemDir.Pictures);
        var path = $"{dir}/FRC-Screenshot-{DateTime.Now.ToFileTime()}.png";
        data.SavePng(path);
    }
}