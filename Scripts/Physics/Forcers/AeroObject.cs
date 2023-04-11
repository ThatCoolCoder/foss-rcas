using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    [Tool]
    public class AeroObject : AbstractSpatialFluidForcer
    {
        // Like an AeroSurface but for other aerodynamic entities - Body, landing gear, etc.
        // Stuff that generally produces more drag than lift

        [Export] public bool HasLift { get; set; } = true;
        [Export] public AeroValueCube LiftCube { get; set; }
        [Export] public bool HasDrag { get; set; } = true;
        [Export] public AeroValueCube DragCube { get; set; }

        public override void _Ready()
        {
            {
                // Setup defaults for the cubes since c# resources are a little janky

                var defaultCube = ResourceLoader.Load<AeroValueCube>("res://Resources/DefaultAeroValueCube.tres");

                if (LiftCube == null) LiftCube = defaultCube.Duplicate() as AeroValueCube;
                if (DragCube == null) DragCube = defaultCube.Duplicate() as AeroValueCube;
            }
            if (Engine.EditorHint) return;

            UpdateDebugBoxVisibility();
            onDebugModeChanged += UpdateDebugBoxVisibility;
            base._Ready();
        }

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState state)
        {
            var totalForce = Vector3.Zero;

            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalTranslation);

            var basis = GlobalTransform.basis;
            basis.Scale = Vector3.One;
            // Velocity relative to the rotation of self
            var localVelocity = basis.XformInv(relativeVelocity);

            var size = Scale;

            if (HasLift)
            {
                // totalForce += Calculate
            }
            if (HasDrag)
            {
                var frontalArea = InterpolateValueFromCube(localVelocity, AeroValueCube.FromVector3(Size));
                var coefficient = InterpolateValueFromCube(localVelocity, DragCube);
                float dragMag = 0.5f * coefficient * frontalArea * localVelocity.LengthSquared();
                totalForce += dragMag * localVelocity.Normalized() * -1;
            }
            return totalForce;
        }

        private float InterpolateValueFromCube(Vector3 localVelocity, AeroValueCube aeroValueCube)
        {
            // Interpolate between the different directions within the cube

            float InterpolatePositiveNegative(float speedProportion, float negativeCoefficient, float positiveCoefficient)
            {
                return Mathf.Abs(speedProportion) * (speedProportion > 0 ? positiveCoefficient : negativeCoefficient);
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
