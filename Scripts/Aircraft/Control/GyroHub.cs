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

        private float previousError = 0;
        private float integralError = 0;

        public float CalculateOutput(float currentValue, float targetValue, float deltaTime)
        {
            var error = targetValue - currentValue;

            integralError += error * deltaTime;

            var derivative = (error - previousError) / deltaTime;

            previousError = error;

            var raw = P * error +
                I * integralError +
                D * derivative;

            return Mathf.Clamp(raw, -1, 1);
        }
    }

    public class GyroSettings
    {
        public PidTuning PitchTuning = new();
        public float PitchRateDegrees = 500;
        public PidTuning RollTuning = new();
        public float RollRateDegrees = 500;
        public PidTuning YawTuning = new();
        public float YawRateDegrees = 500;
    }

    public class GyroHub : MixerHub
    {
        // todo: Hacky implementation, need to rework the base class to make it more extensible

        [Export(PropertyHint.File, "*.toml")] public string GyroSettingsFile { get; set; }
        private GyroSettings gyroSettings;

        [Export] public NodePath RigidBodyPath { get; set; }
        private RigidBody rigidBody;


        public override void _Ready()
        {
            base._Ready();

            rigidBody = Utils.GetNodeWithWarnings<RigidBody>(this, RigidBodyPath, "rigidbody", true);

            var gdFile = new File();
            gdFile.Open(GyroSettingsFile, File.ModeFlags.Read);
            var content = gdFile.GetAsText();
            gdFile.Close();
            gyroSettings = TomletMain.To<GyroSettings>(content);
        }

        public override void _Process(float delta)
        {
            var angularVelocity = rigidBody.AngularVelocity;
            angularVelocity = GlobalTransform.basis.Inverse().Xform(angularVelocity);
            var elevatorValue = gyroSettings.PitchTuning.CalculateOutput(SimInput.Manager.GetActionValue("aircraft/elevator") * gyroSettings.PitchRateDegrees,
                Mathf.Rad2Deg(angularVelocity.x),
                delta);

            var aileronValue = gyroSettings.RollTuning.CalculateOutput(SimInput.Manager.GetActionValue("aircraft/aileron") * gyroSettings.RollRateDegrees,
                Mathf.Rad2Deg(angularVelocity.z),
                delta);

            var rudderValue = gyroSettings.YawTuning.CalculateOutput(SimInput.Manager.GetActionValue("aircraft/rudder") * gyroSettings.YawRateDegrees,
                Mathf.Rad2Deg(angularVelocity.y),
                delta);

            var newChannelValues = new Dictionary<string, float>();
            foreach (var mix in channelMixSet.Mixes)
            {
                float previousValue = 0;
                newChannelValues.TryGetValue(mix.OutputChannelName, out previousValue);

                var rawValue = SimInput.Manager.GetActionValue("aircraft/" + mix.InputChannelName);
                if (GetNode<Timer>("Timer").TimeLeft == 0)
                {
                    if (mix.InputChannelName == "elevator") rawValue = elevatorValue;
                    if (mix.InputChannelName == "aileron") rawValue = aileronValue;
                    if (mix.InputChannelName == "rudder") rawValue = rudderValue;
                }
                newChannelValues[mix.OutputChannelName] = mix.Apply(rawValue, previousValue, delta);
            }

            ChannelValues = newChannelValues.ToDictionary(kvp => kvp.Key, kvp => Mathf.Clamp(kvp.Value, -1, 1));
        }
    }
}