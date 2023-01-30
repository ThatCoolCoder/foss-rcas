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
        [Export] public bool FreeWheelWhenOff { get; set; } // If this is true, motor will not generate any drag when thrustproportion = 0
        public float lastExitSpeed { get; private set; } // last exit speed, relative to world

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState state)
        {
            if (ThrustProportion == 0 && FreeWheelWhenOff) return Vector3.Zero;

            var density = fluid.DensityAtPoint(GlobalTransform.origin);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalTransform.origin);
            var localVelocity = GlobalTransform.basis.XformInv(relativeVelocity);
            var entrySpeed = localVelocity.z;
            var effectiveExitSpeed = ThrustProportion * ExitSpeed;
            var deltaSpeed = effectiveExitSpeed - entrySpeed;

            lastExitSpeed = deltaSpeed;

            var area = Mathf.Pi * Radius * Radius;

            var velocityAtDisk = .5f * (entrySpeed + effectiveExitSpeed);

            var force = density * velocityAtDisk * area * deltaSpeed;

            return GlobalTransform.basis.z * force;
        }
    }
}