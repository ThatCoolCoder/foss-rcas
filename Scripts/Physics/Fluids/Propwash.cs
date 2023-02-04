using Godot;
using System;

namespace Physics.Fluids
{
    public class Propwash : Spatial, ISpatialFluid
    {
        // Class that adds propwash to a motor. Motor must be the parent of this

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

        private Forcers.Motor motor;

        public override void _Ready()
        {
            try
            {
                motor = GetParent<Forcers.Motor>();
            }
            catch (InvalidCastException)
            {
                Utils.LogError($"The parent of {Name} was not a Motor", this);
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
            var delta = point - motor.GlobalTranslation;
            var directionToPoint = delta.Normalized();
            var speedMultiplier = delta.LengthSquared() / MaxDistance;
            var velocity = directionToPoint * speedMultiplier * Mathf.Max(motor.lastExitSpeed, 0); // don't let the wash go backwards
            return velocity + motor.lastEntryVelocity;
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