using Godot;
using System;

namespace Aircraft
{
    public class Servo : Spatial, Control.IControllable
    {
        // Object that rotates around its X-axis

        [Export] public float MaxDeflectionDegrees { get; set; } = 30;
        [Export] public bool Reversed { get; set; }
        [Export] public string ChannelName { get; set; } = "";
        [Export] public float Time60Degrees { get; set; } = 0.10f;

        public Control.IHub ControlHub { get; set; }
        private float trueDeflection = 0;
        private float targetDeflection = 0;

        public override void _Process(float delta)
        {
            targetDeflection = ControlHub.ChannelValues[ChannelName];
            targetDeflection *= Mathf.Deg2Rad(MaxDeflectionDegrees);
            if (Reversed) targetDeflection *= -1;

            Rotation = Rotation.WithX(trueDeflection);
        }

        public override void _PhysicsProcess(float delta)
        {
            var rotationSpeed = Mathf.Deg2Rad(60.0f / Time60Degrees);
            trueDeflection = Utils.ConvergeValue(trueDeflection, targetDeflection, rotationSpeed * delta);
        }
    }
}