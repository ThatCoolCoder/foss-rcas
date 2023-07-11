using Godot;
using System;

namespace Audio;

public partial class PrerecordedPropellerNoise : AudioStreamPlayer3D
{
    [Export] public float DominantFrequency { get; set; }
    [Export] public float DecibelsChange { get; set; } = 40;
    [Export] public int BladeCount { get; set; } = 2;
    [Export] public NodePath PropellerPath { get; set; }
    private Physics.Forcers.Propeller propeller { get; set; }
    private float initialUnitDb;

    public override void _Ready()
    {
        propeller = Utils.GetNodeWithWarnings<Physics.Forcers.Propeller>(this, PropellerPath, "propeller", true);
        initialUnitDb = VolumeDb;
    }

    public override void _Process(double delta)
    {
        var rpmInRecording = DominantFrequency / BladeCount * 60;

        var targetPitchScale = Mathf.Abs(propeller.Rpm) / rpmInRecording;
        if (targetPitchScale <= 0)
        {
            // Streams don't appreciate pitch scale being set to 0, so just pause them in that case
            StreamPaused = true;
        }
        else
        {
            PitchScale = targetPitchScale;
            StreamPaused = false;
        }
        VolumeDb = initialUnitDb - (1 - targetPitchScale) * DecibelsChange;
    }
}