using Godot;
using System;

namespace Audio;

public partial class PrerecordedPropellerNoise : AudioStreamPlayer3D
{
    [Export] public float DominantFrequencyInRecording { get; set; } = 345;
    [Export] public float InitialVolume { get; set; } = 0;
    [Export] public float RpmOfInitialVolume { get; set; } = 12345;
    [Export] public float DecibelsChangePer1000Rpm { get; set; } = -2;
    [Export] public int BladeCount { get; set; } = 2;
    [Export] public NodePath PropellerPath { get; set; }
    private Physics.Forcers.Propeller propeller { get; set; }
    private float initialUnitDb;

    public override void _Ready()
    {
        StreamPaused = true;
        propeller = Utils.GetNodeWithWarnings<Physics.Forcers.Propeller>(this, PropellerPath, "propeller", true);
    }

    public override void _Process(double delta)
    {
        var rpmInRecording = DominantFrequencyInRecording / BladeCount * 60;

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
        var volumeOffset = (RpmOfInitialVolume - propeller.Rpm) / 1000 * DecibelsChangePer1000Rpm;
        VolumeDb = InitialVolume + volumeOffset;
    }
}