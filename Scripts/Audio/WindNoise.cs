using Godot;
using System;

namespace Audio;

public partial class WindNoise : ProceduralSpatialAudio
{
    // sorry this is very messy due to the process of experimentation - it's basically an auditory shader, what did you expect?

    [Export] public float VolumeMultiplier { get; set; } = 0.1f;
    [Export] public NodePath AirPath { get; set; }
    private Physics.Fluids.Air air;

    private FastNoiseLite volumeNoise = new();
    private float volumeNoiseFrequency = 150;
    private float volumeNoiseFactor = 0.25f;

    private FastNoiseLite pinkNoise = new();

    private float approximatePitch = 50;

    private Vector3 previousPosition;
    private float airSpeed = 0;
    private float leftAudioFactor;
    private float rightAudioFactor;

    private Primitives.SineOscillator sineWave = new() { Frequency = 500 };
    private Primitives.SquareOscillator saw = new() { Frequency = 500 };
    private float sineWaveFactor = 0.05f;

    public override void _Ready()
    {
        air = GetNode<Physics.Fluids.Air>(AirPath);
        base._Ready();

        previousPosition = GetParent<Node3D>().GlobalPosition;

        volumeNoise.FractalOctaves = 1;

        pinkNoise.FractalOctaves = 5;
        pinkNoise.FractalLacunarity = 2;
        pinkNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        pinkNoise.FractalGain = 0.25f;
    }

    protected override Vector2 ComputeAudioValue()
    {
        var baseValue = pinkNoise.GetNoise1D(time * 100000) / 2 + 0.5f;
        var periodicVolumeMultiplier = 1 + volumeNoise.GetNoise1D(volumeNoiseFrequency * time) * volumeNoiseFactor;
        baseValue *= airSpeed;
        baseValue *= VolumeMultiplier * periodicVolumeMultiplier;

        return new Vector2(leftAudioFactor * baseValue, rightAudioFactor * baseValue);
    }

    public override void _Process(double delta)
    {
        var fdelta = (float)delta;
        var globalVelocity = (GlobalPosition - previousPosition) / fdelta;
        var relativeVelocity = globalVelocity - air.VelocityAtPoint(GlobalPosition);
        var localVelocity = GlobalTransform.Basis.Transposed() * relativeVelocity;

        // Might as well calculate all this in process and not the audio value computer since velocity only makes sense to update that fast anyway
        airSpeed = localVelocity.Length();
        var airDirection = new Vector2(localVelocity.X, localVelocity.Z).Angle();
        airDirection += Mathf.Pi / 2;
        leftAudioFactor = Mathf.Sin(-airDirection) / 2 + 0.5f;
        rightAudioFactor = Mathf.Sin(airDirection) / 2 + 0.5f;

        // GlobalPosition = GetParent<Spatial>().GlobalPosition + relativeVelocity.Normalized();

        previousPosition = GlobalPosition;
        base._Process(delta);
    }
}
