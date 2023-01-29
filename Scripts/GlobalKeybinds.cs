using Godot;
using System;

public class GlobalKeybinds : Node
{
    // Class handling global (as in throughout the entire game) keybinds

    public override void _Process(float delta)
    {
        // This one is nice to have global so we can trigger it before loading in the physics stuff (which may only check it on _ready)
        if (Input.IsActionJustPressed("toggle_physics_debug")) Physics.Forcers.AbstractSpatialFluidForcer.DebugModeActive = !Physics.Forcers.AbstractSpatialFluidForcer.DebugModeActive;
        if (Input.IsActionJustPressed("complete_reset")) GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
    }
}