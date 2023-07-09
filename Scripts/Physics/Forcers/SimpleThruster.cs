using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public partial class SimpleThruster : AbstractSpatialForcer
    {
        [Export] public float MaxThrust { get; set; } = 10;
        [Export] public float MaxSpeed { get; set; } = 30; // thrust decays to 0 at this speed

        [Export] public float ThrustProportion { get; set; } = 0; // public so people can mess with it in content creation tutorial before learning about control

        public override void Apply(PhysicsDirectBodyState3D state)
        {
            ThrustProportion = Mathf.Clamp(ThrustProportion, -1, 1);

            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this);
            var localVelocity = GlobalTransform.Basis.Inverse() * relativeVelocity;

            var forceMag = Utils.MapNumber(-localVelocity.Z, 0, MaxSpeed, MaxThrust, 0) * ThrustProportion;
            var force = -GlobalTransform.Basis.Z * forceMag;

            state.AddConstantForce(force, GlobalPosition - state.Transform.Origin);
        }
    }
}