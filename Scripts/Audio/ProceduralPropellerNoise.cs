using Godot;
using System;

namespace Audio
{
    public partial class ProceduralPropellerNoise : ProceduralSpatialAudio
    {
        // sorry this is very messy due to the process of experimentation - it's basically an auditory shader, what did you expect?

        [Export] public float VolumeMultiplier { get; set; } = 0.1f;

        [Export] public NodePath propellerPath;
        private Physics.Forcers.Propeller propeller;


        private FastNoiseLite pinkNoise = new();
        private float approximatePitch = 50;

        private float maxRpm = 25000;
        private float blades = 2;
        private float bps = 0; // blades per second

        private Primitives.SineOscillator sineWave = new() { Frequency = 500 };
        private Primitives.SawtoothOscillator saw = new() { Frequency = 500 };
        private float sineWaveFactor = 0.05f;

        public override void _Ready()
        {
            propeller = GetNode<Physics.Forcers.Propeller>(propellerPath);

            base._Ready();


            pinkNoise.FractalOctaves = 5;
            pinkNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
            pinkNoise.FractalLacunarity = 2;
            pinkNoise.FractalGain = 0.25f;
        }

        protected override Vector2 ComputeAudioValue()
        {
            // if (motor == null) return Vector2.Zero;

            var pink = pinkNoise.GetNoise1D(time * 100000) / 2 + 0.5f;
            saw.Frequency = bps;
            var sawn = saw.Sample(sampleHz);
            sawn *= Mathf.PosMod(time, 1 / bps) * bps;
            var baseValue = pink * .5f + sawn * .5f;

            baseValue *= VolumeMultiplier;

            return Vector2.One * baseValue;
        }

        public override void _Process(double delta)
        {
            bps = Mathf.Abs(propeller.AngularVelocity) / Mathf.Tau * 2;
            base._Process(delta);
        }
    }
}
