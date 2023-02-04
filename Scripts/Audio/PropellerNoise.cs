using Godot;
using System;

namespace Audio
{
    public class PropellerNoise : ProceduralSpatialAudio
    {
        // sorry this is very messy due to the process of experimentation - it's basically an auditory shader, what did you expect?

        [Export] public float VolumeMultiplier { get; set; } = 0.1f;

        [Export] public NodePath motorPath;
        private Physics.Forcers.Motor motor;


        private OpenSimplexNoise pinkNoise = new();
        private float approximatePitch = 50;

        private float maxRpm = 25000;
        private float blades = 2;
        private float bps = 0; // blades per second

        private Primitives.SineOscillator sineWave = new() { Frequency = 500 };
        private Primitives.SawtoothOscillator saw = new() { Frequency = 500 };
        private float sineWaveFactor = 0.05f;

        public override void _Ready()
        {
            motor = GetNode<Physics.Forcers.Motor>(motorPath);

            base._Ready();


            pinkNoise.Octaves = 5;
            pinkNoise.Lacunarity = 2;
            pinkNoise.Persistence = 0.25f;
        }

        protected override Vector2 ComputeAudioValue()
        {
            // if (motor == null) return Vector2.Zero;

            var pink = pinkNoise.GetNoise1d(time * 100000) / 2 + 0.5f;
            saw.Frequency = bps;
            var sawn = saw.Sample(sampleHz);
            sawn *= Mathf.PosMod(time, 1 / bps) * bps;
            var baseValue = pink * .5f + sawn * .5f;

            baseValue *= VolumeMultiplier;

            return Vector2.One * baseValue;
        }

        public override void _Process(float delta)
        {
            bps = Mathf.Abs(motor.ThrustProportion) * maxRpm / 60.0f * blades;
            base._Process(delta);
        }
    }
}
