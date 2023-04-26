using Godot;
using System;

namespace Physics.Fluids
{
    public class Propwash : Spatial, ISpatialFluid
    {
        // Class that adds propwash to a propeller. Must be the child of the propeller.
        // May be inaccurate for objects very close to the propeller since it assumes propwash is a perfect cone.

        [Export] public float MaxDistance { get; set; } = 2;
        [Export]
        public float SpreadAngleDegrees
        {
            get
            {
                return Mathf.Rad2Deg(spreadAngle);
            }
            set
            {
                spreadAngle = Mathf.Deg2Rad(value);
            }
        }
        private float spreadAngle = Mathf.Deg2Rad(20);

        private Forcers.Propeller propeller;

        public override void _Ready()
        {
            try
            {
                propeller = GetParent<Forcers.Propeller>();
            }
            catch (InvalidCastException)
            {
                Utils.LogError($"The parent of {Name} was not a Propeller", this);
            }
        }

        public float DensityAtPoint(Vector3 _point)
        {
            return 1.293f; // todo: should this have some way of tapping into what fluid is in the surroundings, to get the right density?
        }

        public bool ContainsPoint(Vector3 point)
        {
            return (GlobalTranslation.DistanceSquaredTo(point) < MaxDistance * MaxDistance &&
                AngleToPoint(point) < spreadAngle);
        }

        public Vector3 VelocityAtPoint(Vector3 point)
        {
            var localPosition = ToLocal(point);

            // Make speed fall off further away from prop
            var axialSpeedMultiplier = Mathf.Abs(localPosition.z) / MaxDistance;
            AngleToPoint(point);

            // Make speed higher at the outside, since the blades spin faster there.
            var radiusAtDistance = Mathf.Tan(spreadAngle) * localPosition.z;
            var radialSpeedMultiplier = new Vector2(localPosition.x, localPosition.y).Length() / radiusAtDistance;
            radialSpeedMultiplier = Mathf.Max(radialSpeedMultiplier, .25f);

            var directionToPoint = localPosition.Normalized();
            var velocity = directionToPoint * axialSpeedMultiplier * radialSpeedMultiplier * Mathf.Max(propeller.LastExitSpeed, 0); // don't let the wash go backwards
            return velocity + propeller.LastEntryVelocity;
        }

        public Vector3 NormalAtPoint(Vector3 _point)
        {
            return Vector3.Up;
        }

        private float AngleToPoint(Vector3 globalPoint)
        {
            var local = ToLocal(globalPoint);
            var squashed = new Vector2(local.x, local.y);
            var sideDisplacement = squashed.Length();
            return Mathf.Atan2(sideDisplacement, local.z);
        }

        public FluidType Type { get; set; } = FluidType.Gas;
    }
}