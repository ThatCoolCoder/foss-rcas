using Godot;
using System;

public partial class AeroTestScene : Node3D
{
    public override void _Ready()
    {
        SimSettings.Settings.LoadCurrent();
        SimSettings.Settings.Current.ApplyToViewport(GetViewport() as SubViewport);
        SimInput.Manager.Instance.LoadInputMap(SimSettings.Settings.Current.InputMap);
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.R))
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
