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
        [Export] public AeroCoefficientCube LiftCube { get; set; }
        [Export] public bool HasDrag { get; set; } = true;
        [Export] public AeroCoefficientCube DragCube { get; set; }

        public override void _Ready()
        {
            var defaultCube = ResourceLoader.Load<AeroCoefficientCube>("res://Resources/DefaultAeroCoefficientCube.tres");

            if (LiftCube == null) LiftCube = defaultCube.Duplicate() as AeroCoefficientCube;
            if (DragCube == null) DragCube = defaultCube.Duplicate() as AeroCoefficientCube;

            if (Engine.EditorHint) return;

            UpdateDebugBoxVisibility();
            onDebugModeChanged += UpdateDebugBoxVisibility;
            base._Ready();
        }

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState state)
        {
            return Vector3.Zero;
        }

        private float InterpolateCoefficientFromCube(Vector3 localVelocity, AeroCoefficientCube aeroCoefficientCube)
        {
            // Interpolate between the different directions within the cube

            float InterpolatePositiveNegative(float speedProportion, float negativeCoefficient, float positiveCoefficient)
            {
                return Mathf.Abs(speedProportion) * (speedProportion > 0 ? positiveCoefficient : negativeCoefficient);
            }

            var normalised = localVelocity.Normalized();

            return InterpolatePositiveNegative(normalised.x, aeroCoefficientCube.Left, aeroCoefficientCube.Right) +
                InterpolatePositiveNegative(normalised.y, aeroCoefficientCube.Down, aeroCoefficientCube.Up) +
                InterpolatePositiveNegative(normalised.z, aeroCoefficientCube.Forward, aeroCoefficientCube.Back);
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
