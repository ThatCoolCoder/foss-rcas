using Godot;
using System;

public class AeroTestScene : Spatial
{
    public override void _Ready()
    {
        SimSettings.Settings.LoadCurrent();
        SimInput.Manager.ApplyAxisMappings();
    }
}
