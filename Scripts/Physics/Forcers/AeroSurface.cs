using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers;

public partial class AeroSurface : AbstractSpatialFluidForcer
{
    // Basically a wing, not restricted to operating in air though.
    // Uses a model supporting lift, induced drag, and parasitic drag.
    // Wing is oriented along the x/z plane, and forward is in the direction of z-negative (indicated by green arrow)
    // Area/size is determined by the scale of the entity in the scene, although there is also an area multiplier.
    // The multiplier's main use is to allow you to scale this to match size of a non-square object but still have correct area. EG use 0.5 for a triangle.

    // Angle of attack ranges from 0 to tau, and should be normalised to the correct range by wrapping around.

    // Curves have values from 0 to 1, where 0 is -90° aoa and 1 is 90° aoa. Remaining values are interpolated by assuming the surface is a flat plate when run backwards.
    // It's recommended to try editing the resource files for the curves manually, as you can't get very precise results using godot's built-in editor (we should get a custom curve format)
    // The x-value for an aoa can be calculated like so: (aoa / 180 + 0.5)

    // Total lift points perpendicular to surface and causes both true lift and induced drag.
    // For regions where downwards lift is generated, the curve needs to have a sub-zero value
    [Export] public Curve TotalLiftCoefficient { get; set; }
    [Export] public Curve ParasiticDragCoefficient { get; set; }
    [Export] public float AreaMultiplier { get; set; } = 1;


    private float area
    {
        get
        {
            return Mathf.Abs(Scale.X * Scale.Z * AreaMultiplier);
        }
    }

    private static readonly Curve flatPlateLiftCoefficient = ResourceLoader.Load<Curve>("res://Resources/AeroCurves/FlatPlateLiftCoefficient.tres");

    // Thickness of the wing - used for parasitic drag calculations
    [Export] public float Thickness { get; set; }

    public override void _Ready()
    {
        UpdateDebugBoxVisibility();
        onDebugModeChanged += UpdateDebugBoxVisibility;
        base._Ready();
    }

    public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState3D state)
    {
        var density = fluid.DensityAtPoint(GlobalPosition);
        var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalPosition);

        var basis = GlobalTransform.Basis.Orthonormalized();
        // Velocity relative to the rotation of self
        var localVelocity = basis.Transposed() * relativeVelocity;

        // Regular lift (modelled as airfoil)
        var regularLocalVelocity = localVelocity.WithX(0);
        var regularAoa = WrapAoa(Mathf.Atan2(-regularLocalVelocity.Y, -regularLocalVelocity.Z));
        var regularLiftVector = CalculateLiftForce(basis, regularLocalVelocity.LengthSquared(), regularAoa, TotalLiftCoefficient, ParasiticDragCoefficient, density);

        // Spanwise lift (modelled as a flat plate)
        var spanwiseLocalVelocity = localVelocity.WithZ(0);
        var spanwiseAoa = WrapAoa(Mathf.Atan2(-spanwiseLocalVelocity.Y, spanwiseLocalVelocity.X));
        var spanwiseLiftVector = CalculateLiftForce(basis, spanwiseLocalVelocity.LengthSquared(), spanwiseAoa, flatPlateLiftCoefficient, ParasiticDragCoefficient, density);

        var liftVector = regularLiftVector + spanwiseLiftVector;

        var dragVector = CalculateDragForce(basis, localVelocity, relativeVelocity, density);

        if (DebugModeActive)
        {
            DebugLineDrawer.RegisterLineStatic(this, GlobalPosition, GlobalPosition + liftVector, Colors.Blue, 1);
            DebugLineDrawer.RegisterLineStatic(this, GlobalPosition, GlobalPosition + dragVector, Colors.Red, 2);
        }

        // return liftVector + dragVector;
        return liftVector;
    }

    private Vector3 CalculateLiftForce(Basis basis, float localSpeedSquared, float aoa, Curve totalLiftCurve, Curve parasiticDragCurve, float fluidDensity)
    {
        var liftCoefficient = InterpolateFromHalfAoaCurve(totalLiftCurve, aoa, flatPlateLiftCoefficient);
        var liftMag = CalculateAeroForceMagnitude(liftCoefficient, area, fluidDensity, localSpeedSquared);
        var liftVector = basis.Y * liftMag;

        return liftVector;
    }

    private Vector3 CalculateDragForce(Basis basis, Vector3 localVelocity, Vector3 relativeVelocity, float fluidDensity)
    {
        var aoa = CalculateAngleOfAttack(localVelocity);
        var localSpeedSquared = localVelocity.LengthSquared();
        var parasiticDragCoefficient = InterpolateFromHalfAoaCurve(ParasiticDragCoefficient, aoa);
        var frontalArea = CalculateFrontalArea(aoa);
        var dragMag = CalculateAeroForceMagnitude(parasiticDragCoefficient, frontalArea, fluidDensity, localSpeedSquared);
        return -relativeVelocity.Normalized() * dragMag;
    }

    private float CalculateAngleOfAttack(Vector3 localVelocity)
    {
        // Calculate AOA as if the surface is a round flat plate, working equally well in all directions.
        // If that assumption is not valid for some calculation, calculate AOA yourself.

        // Probably could do this better with quaternions but this works...
        var tr = new Transform3D().LookingAt(localVelocity, Vector3.Up);
        var euler = tr.Basis.GetEuler();
        return WrapAoa(-euler.X);
    }

    private float WrapAoa(float rawAoa)
    {
        // Wrap a raw aoa value to range of 0 to Tau
        return Utils.WrapNumber(rawAoa, 0, Mathf.Tau);
    }

    private float CalculateAeroForceMagnitude(float coefficient, float area, float density, float speedSquared)
    {
        // Since lift and drag equations are so similar, we can have one method for calculating both.

        return coefficient * area * density * speedSquared / 2;
    }

    private float InterpolateFromHalfAoaCurve(Curve curve, float aoa, Curve backwardFlightCurve = null)
    {
        // Interpolate a value, by presuming that the surface is symmetrical
        // Alternatively you can supply an extra curve to use when the surface moves backwards
        // Presumes that the aoa has already been normalised

        if (backwardFlightCurve == null) backwardFlightCurve = curve;


        float deg90 = Mathf.Pi / 2;

        // Forward flight
        if (aoa < deg90 || aoa > deg90 * 3)
        {
            if (aoa > Mathf.Pi) aoa -= Mathf.Tau;
            return curve.Sample(Utils.MapNumber(aoa, -deg90, deg90, 0, 1));
        }
        // Backward flight
        {
            return backwardFlightCurve.Sample(Utils.MapNumber(aoa, deg90, deg90 * 3, 1, 0));
        }
    }

    private float CalculateFrontalArea(float aoa)
    {
        // Calculate the frontal area for a given aoa, for parastic drag calculations

        // Make there be 1 axis of symmetry
        if (aoa > Mathf.Pi) aoa -= Mathf.Pi;

        var frontLipThickness = Thickness * Mathf.Cos(aoa);
        var frontLipArea = Mathf.Abs(frontLipThickness * Mathf.Sqrt(area));
        var bodyArea = Mathf.Sin(aoa) * area;

        return frontLipArea + bodyArea;
    }

    private void UpdateDebugBoxVisibility()
    {
        GetNode<Node3D>("DebugBox").Visible = DebugModeActive;
    }

    public override void _ExitTree()
    {
        onDebugModeChanged -= UpdateDebugBoxVisibility;
    }
}
