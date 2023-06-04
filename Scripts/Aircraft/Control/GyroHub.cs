using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

using Tomlet;

namespace Aircraft.Control
{
    public class PidTuning
    {
        public float P = 1;
        public float I = 1;
        public float D = 1;
    }

    public class GyroSettings
    {
        public PidTuning PitchTuning = new();
        public PidTuning YawTuning = new();
        public PidTuning RollTuning = new();
    }

    public class GyroHub : Hub
    {
        [Export(PropertyHint.File, "*.toml")] public string GyroSettingsFile { get; set; }
        private GyroSettings gyroSettings;

        [Export] public NodePath RigidBodyPath { get; set; }
        private RigidBody rigidBody;


        public override void _Ready()
        {
            base._Ready();

            rigidBody = Utils.GetNodeWithWarnings<RigidBody>(this, RigidBodyPath, "rigidbody", true);

            var gdFile = new File();
            gdFile.Open(MixesFile, File.ModeFlags.Read);
            var content = gdFile.GetAsText();
            gdFile.Close();
            gyroSettings = TomletMain.To<GyroSettings>(content);

        }

        public override void _Process(float delta)
        {
            var newChannelValues = new Dictionary<string, float>();

            foreach (var mix in channelMixSet.Mixes)
            {
                float previousValue = 0;
                newChannelValues.TryGetValue(mix.OutputChannelName, out previousValue);

                var rawValue = SimInput.Manager.GetAxisValue(mix.InputChannelName);
                newChannelValues[mix.OutputChannelName] = mix.Apply(rawValue, previousValue, delta);
            }

            ChannelValues = newChannelValues.ToDictionary(kvp => kvp.Key, kvp => Mathf.Clamp(kvp.Value, -1, 1));
        }
    }
}