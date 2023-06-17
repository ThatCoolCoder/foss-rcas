using Godot;
using System;

namespace Locations
{
    public class WalkingGroundCamera : GroundCamera
    {
        [Export] public bool IsFreeMode { get; set; } = false;

        public override void _Ready()
        {
            if (TargetPath != null) Target = GetNode<Spatial>(TargetPath);
            base._Ready();
        }

        public override void _Process(float delta)
        {
            if (!Enabled) return;

            if (SimInput.Manager.IsActionJustPressed("camera/reset")) IsFreeMode = false;

            // if is angle input then become free mode

            if (IsFreeMode || Target == null)
            {
                // have drag input, have custom input, have movement.
            }
            else
            {
                base._Process(delta);
            }
        }
    }
}
