using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers;

public partial class AeroObject : AbstractSpatialFluidForcer
{
    // Like an AeroSurface but for other aerodynamic entities - Body, landing gear, etc.
    // Stuff that generally produces more drag than lift
    // Set cubes to null to have no effect
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
        var (relativeVelocity, localVelocity, basis) = GetRelativeVelLocalVelUsableBasis(fluid, state);

        var size = Scale;
        var sideAreas = new Vector3(size.Y * size.Z, size.X * size.Z, size.X * size.Y);

        if (LiftCube != null)
        {
            // Calculate force for the 3 axis separately then combine.

            var localLift = new Vector3(
                GetLiftAlongAxis(localVelocity.X, localVelocity.Z, localVelocity.Y, LiftCube.Left, LiftCube.Right) * sideAreas.X,
                GetLiftAlongAxis(localVelocity.Y, localVelocity.X, localVelocity.Z, LiftCube.Down, LiftCube.Up) * sideAreas.Y,
                GetLiftAlongAxis(localVelocity.Z, localVelocity.X, localVelocity.Y, LiftCube.Forward, LiftCube.Back) * sideAreas.Z);
            var relativeLift = basis * localLift;
            totalForce += relativeLift * 0.2f; // for some reason the lift is way strong so let's just cut it down here
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

    private float GetLiftAlongAxis(float perpendicularVelocity, float acrossVelocity1, float acrossVelocity2, float negativeCoefficient, float positiveCoefficient)
    {
        // Across velocity 1 and 2 are the two components of the velocity across the face. EG if perpendicular is the Z then across1 is x and 2 is y

        var across2d = new Vector2(acrossVelocity1, acrossVelocity2);
        var alpha = Mathf.RadToDeg(Mathf.Atan(Mathf.Abs(perpendicularVelocity) / across2d.Length()));
        var aoaMult = alpha < 30 ? Utils.MapNumber(alpha, 0, 30, 0, 1) : Utils.MapNumber(alpha, 30, 90, 1, 0);
        var direction = Mathf.Sign(perpendicularVelocity) * -1;
        return across2d.LengthSquared() * (perpendicularVelocity > 0 ? positiveCoefficient : negativeCoefficient) * aoaMult * direction;
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
