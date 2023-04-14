using Godot;
using System;

public class AeroTestScene : Spatial
{
    public override void _Ready()
    {
        SimSettings.Settings.LoadCurrent();
    }

    public override void _Process(float delta)
    {
        if (Input.IsKeyPressed((int)KeyList.R))
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
