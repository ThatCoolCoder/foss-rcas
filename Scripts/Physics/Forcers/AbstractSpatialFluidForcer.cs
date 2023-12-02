using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Physics.Forcers;

public abstract partial class AbstractSpatialFluidForcer : AbstractSpatialForcer
{
    // Base class for things that apply force because of fluids.
    // Note that this is tool-safe: extending classes can be [Tool]s without issues


    // If this is true, does not run CalculateForce on fluids that it's not within. Set this to false if you care about the edges of fluids (eg floating)
    protected virtual bool autoCheckInsideFluid { get; set; } = true;
    private List<Fluids.ISpatialFluid> fluids;

    public static bool DebugModeActive { get; private set; } = (bool)ProjectSettings.GetSetting("global/physics_debug_active");
    protected static event Action onDebugModeChanged;
    private bool debugModeWasActive = false;


    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;

        base._Ready();

        fluids = Fluids.SpatialFluidRepository.FindSpatialFluidRepository(GetTree()).Fluids;

        debugModeWasActive = DebugModeActive;
    }

    // Should return a force in global coordinates
    public abstract Vector3 CalculateForce(Fluids.ISpatialFluid fluid, PhysicsDirectBodyState3D state);

    public override void Apply(PhysicsDirectBodyState3D state)
    {
        var candidateFluids = fluids.AsEnumerable();
        if (autoCheckInsideFluid)
        {
            candidateFluids = candidateFluids.Where(f => f.ContainsPoint(GlobalPosition));
        }
        var totalForce = candidateFluids.Select(f => CalculateForce(f, state))
            .Aggregate(Vector3.Zero, (prev, next) => prev + next);
        var position = GlobalPosition;
        if (debugModeWasActive) DebugLineDrawer.ClearLinesStatic(this);
        if (DebugModeActive)
        {
            DebugLineDrawer.RegisterLineStatic(this, GlobalPosition, GlobalPosition + totalForce);
        }

        position -= target.GlobalPosition;
        state.ApplyForce(totalForce, position);

        debugModeWasActive = DebugModeActive;
    }

    public static void SetDebugModeActive(bool newDebugModeActive)
    {
        DebugModeActive = newDebugModeActive;
        if (onDebugModeChanged != null) onDebugModeChanged.Invoke();
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint()) return;

        target.UnregisterForcer(this);
        if (debugModeWasActive) DebugLineDrawer.ClearLinesStatic(this);
    }

    protected (Vector3, Vector3, Basis) GetRelativeVelLocalVelUsableBasis(Fluids.ISpatialFluid fluid, PhysicsDirectBodyState3D state)
    {
        var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalPosition);

        var basis = GlobalTransform.Basis.Orthonormalized();
        // Velocity relative to the rotation of self
        var localVelocity = basis.Transposed() * relativeVelocity;

        return (relativeVelocity, localVelocity, basis);
    }
}