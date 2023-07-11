using Godot;
using System;

namespace Aircraft;

public partial class Servo : Node3D
{
    // Object that rotates around its X-axis

    [Export] public float MaxDeflectionDegrees { get; set; } = 30;
    [Export] public bool Reversed { get; set; }
    [Export] public string ChannelName { get; set; } = "";
    [Export] public float Time60Degrees { get; set; } = 0.10f;
    [Export] public NodePath ControlHubPath { get; set; }
    private Control.IHub controlHub;

    private float trueDeflection = 0;
    private float targetDeflection = 0;

    public override void _Ready()
    {
        controlHub = Utils.GetNodeWithWarnings<Control.IHub>(this, ControlHubPath, "control hub", true);
        base._Ready();
    }

    public override void _Process(double delta)
    {
        targetDeflection = controlHub.ChannelValues[ChannelName];
        targetDeflection *= Mathf.DegToRad(MaxDeflectionDegrees);
        if (Reversed) targetDeflection *= -1;

        Rotation = Rotation.WithX(trueDeflection);
    }

    public override void _PhysicsProcess(double delta)
    {
        var rotationSpeed = Mathf.DegToRad(60.0f / Time60Degrees);
        trueDeflection = Utils.ConvergeValue(trueDeflection, targetDeflection, rotationSpeed * (float)delta);
    }
}