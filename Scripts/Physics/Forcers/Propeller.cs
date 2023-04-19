using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public class Propeller : AbstractSpatialFluidForcer
    {
        [Export] public float Radius { get; set; } = 10 * .5f * .0254f; // (In metres)
        [Export] public float Pitch { get; set; } = 6 * .0254f; // Idealised distance travelled forward per rotation, in metres (second number in the propeller size)
        [Export] public bool Clockwise { get; set; } = true;
        [Export] public bool FreeWheelWhenOff { get; set; } // If this is true, prop will not generate any drag if it is spinning too slowly to generate thrust
        public float rpm { get; set; } // Intended to be set by a motor
        public float lastExitSpeed { get; private set; } // last exit speed, relative to world
        public Vector3 lastEntryVelocity { get; private set; } // hacky thing we need to make propwash work with wind

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState state)
        {
            var rps = rpm / 60;
            var exitSpeed = Pitch * rps;


            var density = fluid.DensityAtPoint(GlobalTransform.origin);
            lastEntryVelocity = fluid.VelocityAtPoint(GlobalTransform.origin);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - lastEntryVelocity;
            var localVelocity = GlobalTransform.basis.XformInv(relativeVelocity);
            var entrySpeed = -localVelocity.z;
            var deltaSpeed = exitSpeed - entrySpeed;

            lastExitSpeed = deltaSpeed;

            var area = Mathf.Pi * Radius * Radius;

            var force = .5f * density * area * (exitSpeed * exitSpeed - entrySpeed * entrySpeed);

            if (force < 0 && FreeWheelWhenOff) force = 0;

            if (!Clockwise) force = -force;
            if (rpm < 0) force = -force;

            return -GlobalTransform.basis.z * force;
        }

        public override void _Process(float delta)
        {
            RotateZ(-rpm / 60 * delta * Mathf.Tau); // (In godot a negative rotation along -z is clockwise)
            base._Process(delta);
        }
    }
}