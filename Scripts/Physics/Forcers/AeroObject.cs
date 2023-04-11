using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public class AeroObject : AbstractSpatialFluidForcer
    {
        // Like an AeroSurface but for other aerodynamic entities - Body, landing gear, etc.
        // Stuff that generally produces more drag than lift
        // Set cube paths to null to have no effect

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

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState state)
        {
            var totalForce = Vector3.Zero;

            var density = fluid.DensityAtPoint(GlobalTranslation);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalTranslation);

            var basis = GlobalTransform.basis;
            basis.Scale = Vector3.One;
            // Velocity relative to the rotation of self
            var localVelocity = basis.XformInv(relativeVelocity);

            var size = Scale;

            if (liftCube != null)
            {
                // Do the 3 visible faces separately then add together.
                // so for each axis, find area (from scale), and coefficient (from local velocity).
                // Then use direction of surface + pos or neg to get local force. Then rotate back to global
                // coefficient will be interpolated between CubeVal and 0. 
                // totalForce += CalculateLift(x) + CalculateLift(y) + CalculateLift(z);
            }
            if (dragCube != null)
            {
                var sideAreas = new Vector3(size.y * size.z, size.x * size.z, size.x * size.y);
                var frontalArea = InterpolateValueFromCube(localVelocity, AeroValueCube.FromVector3(sideAreas));
                var coefficient = InterpolateValueFromCube(localVelocity, dragCube);
                float dragMag = 0.5f * coefficient * frontalArea * localVelocity.LengthSquared() * density;
                totalForce += dragMag * relativeVelocity.Normalized() * -1;
            }
            return totalForce;
        }

        private float InterpolateValueFromCube(Vector3 localVelocity, AeroValueCube aeroValueCube)
        {
            // Interpolate between the different directions within the cube

            float InterpolatePositiveNegative(float speedProportion, float negativeCoefficient, float positiveCoefficient)
            {
                // todo: this would be more accurate if we used trig interpolate
                // Currently a parabolic approximation and that's good enough
                return speedProportion * speedProportion * (speedProportion > 0 ? positiveCoefficient : negativeCoefficient);
            }

            var normalised = localVelocity.Normalized();

            return InterpolatePositiveNegative(normalised.x, aeroValueCube.Left, aeroValueCube.Right) +
                InterpolatePositiveNegative(normalised.y, aeroValueCube.Down, aeroValueCube.Up) +
                InterpolatePositiveNegative(normalised.z, aeroValueCube.Forward, aeroValueCube.Back);
        }

        private void UpdateDebugBoxVisibility()
        {
            GetNode<Spatial>("DebugBox").Visible = DebugModeActive;
        }

        public override void _ExitTree()
        {
            if (Engine.EditorHint) return;

            onDebugModeChanged -= UpdateDebugBoxVisibility;
        }
    }
}
