using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public class Motor : AbstractSpatialFluidForcer
    {
        [Export] public float Radius { get; set; }
        [Export] public float ExitSpeed { get; set; }
        [Export] public float ThrustProportion { get; set; } // -1 to +1
        [Export] public bool FreeWheelWhenOff { get; set; } // If this is true, motor will not generate any drag if it is spinning too slowly to generate thrust
        public float lastExitSpeed { get; private set; } // last exit speed, relative to world
        public Vector3 lastEntryVelocity { get; private set; } // hacky thing we need to make propwash work with wind

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState state)
        {

            var density = fluid.DensityAtPoint(GlobalTransform.origin);
            lastEntryVelocity = fluid.VelocityAtPoint(GlobalTransform.origin);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - lastEntryVelocity;
            var localVelocity = GlobalTransform.basis.XformInv(relativeVelocity);
            var entrySpeed = -localVelocity.z;
            var effectiveExitSpeed = ThrustProportion * ExitSpeed;
            var deltaSpeed = effectiveExitSpeed - entrySpeed;

            lastExitSpeed = deltaSpeed;

            var area = Mathf.Pi * Radius * Radius;

            var velocityAtDisk = .5f * (entrySpeed + effectiveExitSpeed);

            var force = density * velocityAtDisk * area * deltaSpeed;

            if (force < 0 && FreeWheelWhenOff) force = 0;

            return -GlobalTransform.basis.z * force;
        }
    }
}