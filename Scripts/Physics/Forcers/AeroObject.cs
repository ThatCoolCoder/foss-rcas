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

        [Export(PropertyHint.File, "*.tres")] public string LiftCubePath { get; set; } = null;
        [Export(PropertyHint.File, "*.tres")] public string DragCubePath { get; set; } = null;
        private AeroValueCube liftCube { get; set; }
        private AeroValueCube dragCube { get; set; }

        public override void _Ready()
        {
            if (LiftCubePath != null) liftCube = ResourceLoader.Load<AeroValueCube>(LiftCubePath);
            if (DragCubePath != null) dragCube = ResourceLoader.Load<AeroValueCube>(DragCubePath);
            UpdateDebugBoxVisibility();
            onDebugModeChanged += UpdateDebugBoxVisibility;
            base._Ready();
        }

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState3D state)
        {
            return Vector3.Zero; // convtodo: enable this forcer

            var totalForce = Vector3.Zero;

            var density = fluid.DensityAtPoint(GlobalPosition);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalPosition);

            var basis = GlobalTransform.Basis.Orthonormalized();
            // Velocity relative to the rotation of self
            var localVelocity = basis.Inverse() * relativeVelocity;

            var size = Scale;
            var sideAreas = new Vector3(size.Y * size.Z, size.X * size.Z, size.X * size.Y);

            if (liftCube != null)
            {
                // Calculate force for the 3 axis separately then combine.

                var localLift = new Vector3(GetLiftAlongAxis(localVelocity.X, liftCube.Left, liftCube.Right) * sideAreas.X,
                    GetLiftAlongAxis(localVelocity.Y, liftCube.Down, liftCube.Up) * sideAreas.Y,
                    GetLiftAlongAxis(localVelocity.Z, liftCube.Forward, liftCube.Back) * sideAreas.Z);
                var relativeLift = basis * localLift;
                totalForce += relativeLift;
            }
            if (dragCube != null)
            {
                var frontalArea = InterpolateValueFromCube(localVelocity, AeroValueCube.FromVector3(sideAreas));
                var coefficient = InterpolateValueFromCube(localVelocity, dragCube);
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
