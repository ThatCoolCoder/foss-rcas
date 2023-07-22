using Godot;
using System;

namespace Physics.Fluids;

public partial class Propwash : Node3D, ISpatialFluid
{
    // Class that adds propwash to a propeller. Must be the child of the propeller.
    // May be inaccurate for objects very close to the propeller since it assumes propwash is a perfect cone.

    [Export] public float MaxDistance { get; set; } = 2;
    [Export]
    public float SpreadAngleDegrees
    {
        get
        {
            return Mathf.RadToDeg(spreadAngle);
        }
        set
        {
            spreadAngle = Mathf.DegToRad(value);
        }
    }
    private float spreadAngle = Mathf.DegToRad(20);
    [Export] public bool RadialSpeedFalloff { get; set; } = true;

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
        return true;
        return (GlobalPosition.DistanceSquaredTo(point) < MaxDistance * MaxDistance &&
            AngleToPoint(point) < spreadAngle);
    }

    public Vector3 VelocityAtPoint(Vector3 point)
    {
        var localPosition = ToLocal(point);

        // Make speed fall off further away from prop
        var axialSpeedMultiplier = Mathf.Abs(localPosition.Z) / MaxDistance;

        // Make speed higher at the outside, since the blades spin faster there.
        // todo: isn't the exit speed constant across the whole prop due to the twist?
        // That's why this behaviour is currently behind a boolean flag
        float radialSpeedMultiplier = 1;
        if (RadialSpeedFalloff)
        {
            var radiusAtDistance = Mathf.Tan(spreadAngle) * localPosition.Z;
            radialSpeedMultiplier = new Vector2(localPosition.X, localPosition.Y).Length() / radiusAtDistance;
            radialSpeedMultiplier = Mathf.Max(radialSpeedMultiplier, .25f);
        }

        var velocity = GlobalTransform.Basis.Z * axialSpeedMultiplier * radialSpeedMultiplier * Mathf.Max(propeller.LastExitSpeed, 0); // don't let the wash go backwards
        return velocity + propeller.LastEntryVelocity;
    }

    public Vector3 NormalAtPoint(Vector3 _point)
    {
        return Vector3.Up;
    }

    private float AngleToPoint(Vector3 globalPoint)
    {
        var local = ToLocal(globalPoint);
        var squashed = new Vector2(local.X, local.Y);
        var sideDisplacement = squashed.Length();
        return Mathf.Atan2(sideDisplacement, local.Z);
    }

    public Vector3 BoundaryAtPoint(Vector3 point)
    {
        throw new NotImplementedException();
    }

    public bool HasBoundaries { get; set; } = false;
}