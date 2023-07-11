using Godot;
using System;

namespace Aircraft;

public partial class ControlledWheelForcer : Physics.Forcers.WheelForcer
{
    [Export] public string DriveActionName { get; set; }
    [Export] public string BrakeActionName { get; set; }
    [Export] public bool ReversibleDrive { get; set; } // whether can drive forward and reverse
    [Export] public bool Backwards { get; set; }
    [Export] public NodePath ControlHubPath { get; set; }
    private Control.IHub controlHub;

    public override void _Ready()
    {
        controlHub = Utils.GetNodeWithWarnings<Control.IHub>(this, ControlHubPath, "control hub");
        base._Ready();
    }

    public override void _Process(double delta)
    {
        var powerProportion = (DriveActionName == null || DriveActionName == "") ? 0 : controlHub.ChannelValues[DriveActionName];
        if (Backwards) powerProportion *= -1;
        if (!ReversibleDrive) powerProportion = powerProportion / 2 + 0.5f;

        WheelDriveFactor = powerProportion;
        var brakeProportion = (BrakeActionName == null || BrakeActionName == "") ? 0 : controlHub.ChannelValues[BrakeActionName];
        WheelBrakeFactor = Utils.MapNumber(brakeProportion, -1, 1, 0, 1);

        base._Process(delta);
    }
}