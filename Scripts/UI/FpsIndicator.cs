using Godot;
using System;

public partial class FpsIndicator : Label
{
    public override void _Ready()
    {
        Visible = SimSettings.Settings.Current.Graphics.ShowFps;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Visible)
        {
            Text = $"{Engine.GetFramesPerSecond()} FPS";
        }
    }
}
