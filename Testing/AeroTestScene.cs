using Godot;
using System;

public partial class AeroTestScene : Node3D
{
    public override void _Ready()
    {
        SimSettings.Settings.LoadCurrent();
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.R))
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
