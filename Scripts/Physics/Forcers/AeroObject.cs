using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public partial class AeroObject : AbstractSpatialFluidForcer
    {
        // Like an AeroSurface but for other aerodynamic entities - Body, landing gear, etc.
        // Stuff that generally produces more drag than lift
        // Set cube paths to null to have no effect
        // todo: this is an explorative implementation, could do with some refactoring

        [Export] public AeroValueCube LiftCube { get; set; }
        [Export] public AeroValueCube DragCube { get; set; }

        public override void _Ready()
        {
            UpdateDebugBoxVisibility();
            onDebugModeChanged += UpdateDebugBoxVisibility;
            base._Ready();
        }

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState3D state)
        {
            var totalForce = Vector3.Zero;

            var density = fluid.DensityAtPoint(GlobalPosition);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalPosition);

            var basis = GlobalTransform.Basis.Orthonormalized();
            // Velocity relative to the rotation of self
            var localVelocity = basis.Inverse() * relativeVelocity;

            var size = Scale;
            var sideAreas = new Vector3(size.Y * size.Z, size.X * size.Z, size.X * size.Y);

            if (LiftCube != null)
            {
                // Calculate force for the 3 axis separately then combine.

                var localLift = new Vector3(GetLiftAlongAxis(localVelocity.X, LiftCube.Left, LiftCube.Right) * sideAreas.X,
                    GetLiftAlongAxis(localVelocity.Y, LiftCube.Down, LiftCube.Up) * sideAreas.Y,
                    GetLiftAlongAxis(localVelocity.Z, LiftCube.Forward, LiftCube.Back) * sideAreas.Z);
                var relativeLift = basis * localLift;
                totalForce += relativeLift;
            }
            if (DragCube != null)
            {
                var frontalArea = InterpolateValueFromCube(localVelocity, AeroValueCube.FromVector3(sideAreas));
                var coefficient = InterpolateValueFromCube(localVelocity, DragCube);
                float dragMag = 0.5f * coefficient * frontalArea * localVelocity.LengthSquared();
                totalForce += dragMag * relativeVelocity.Normalized() * -1;
            }
            return totalForce * density;
        }

        private float GetLiftAlongAxis(float velocity, float negativeCoefficient, float positiveCoefficient)
        {
            return velocity * velocity * (velocity > 0 ? positiveCoefficient : negativeCoefficient) * Mathf.Sign(velocity) * -1;
        }

        private float InterpolateValueFromCube(Vector3 localVelocity, AeroValueCube aeroValueCube)
        {
            // Interpolate between the different directions within the cube

            float InterpolatePositiveNegative(float speedProportion, float negativeCoefficient, float positiveCoefficient)
            {
                // todo: this would be more accurate if we used trig to interpolate
                // Currently a parabolic approximation and that's good enough
                return speedProportion * speedProportion * (speedProportion > 0 ? positiveCoefficient : negativeCoefficient);
            }

            var normalised = localVelocity.Normalized();

            return InterpolatePositiveNegative(normalised.X, aeroValueCube.Left, aeroValueCube.Right) +
                InterpolatePositiveNegative(normalised.Y, aeroValueCube.Down, aeroValueCube.Up) +
                InterpolatePositiveNegative(normalised.Z, aeroValueCube.Forward, aeroValueCube.Back);
        }

        private void UpdateDebugBoxVisibility()
        {
            GetNode<Node3D>("DebugBox").Visible = DebugModeActive;
        }

        public override void _ExitTree()
        {
            if (Engine.IsEditorHint()) return;

            onDebugModeChanged -= UpdateDebugBoxVisibility;
        }
    }
}
