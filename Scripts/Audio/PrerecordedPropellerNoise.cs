using Godot;
using System;

namespace Audio
{
    public class PrerecordedPropellerNoise : AudioStreamPlayer3D
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
            initialUnitDb = UnitDb;
        }

        public override void _Process(float delta)
        {
            var rpmInRecording = DominantFrequency / BladeCount * 60;
            PitchScale = propeller.Rpm / rpmInRecording;
            UnitDb = initialUnitDb - (1 - PitchScale) * DecibelsChange;
        }
    }
}