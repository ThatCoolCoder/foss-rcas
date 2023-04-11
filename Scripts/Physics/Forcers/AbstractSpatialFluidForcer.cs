using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Physics.Forcers
{
    public abstract class AbstractSpatialFluidForcer : Spatial
    {
        // Base class for things that apply force because of fluids.
        // Note that this is tool-safe: extending classes can be [Tools] without issues

        // Path to the target of this forcer. If parent is a SpatialFluidEffectable and path is null, then parent is used
        [Export] public NodePath TargetPath { get; set; }
        [Export] public bool Enabled { get; set; } = true;
        [Export] public bool ForLiquid { get; set; } = false; // todo: get a proper system for setting the below list from the editor.
        private List<Fluids.FluidType> fluidTypes = new();
        protected SpatialFluidEffectable target { get; private set; }

        // If this is true, does not run CalculateForce on fluids that it's not within. Set this to false if you care about the edges of fluids (eg floating)
        protected bool autoCheckInsideFluid { get; set; } = true;

        public static bool DebugModeActive { get; private set; } = (bool)ProjectSettings.GetSetting("global/physics_debug_active");
        protected static event Action onDebugModeChanged;
        private bool debugModeWasActive = false;

        public override void _Ready()
        {
            if (Engine.EditorHint) return;

            if (GetParent() is SpatialFluidEffectable t && (TargetPath == null || TargetPath == "")) target = t;
            else target = GetNode<SpatialFluidEffectable>(TargetPath);

            target.RegisterForcer(this);

            if (ForLiquid) fluidTypes.Add(Fluids.FluidType.Liquid);
            else fluidTypes.Add(Fluids.FluidType.Gas);

            debugModeWasActive = DebugModeActive;
        }

        // Should return a force in global coordinates
        public abstract Vector3 CalculateForce(Fluids.ISpatialFluid fluid, PhysicsDirectBodyState state);

        public void Apply(PhysicsDirectBodyState state)
        {
            if (!Enabled) return;

            var candidateFluids = target.Fluids.Where(f => fluidTypes.Contains(f.Type));
            if (autoCheckInsideFluid)
            {
                candidateFluids = candidateFluids.Where(f => f.ContainsPoint(GlobalTranslation));
            }
            var totalForce = candidateFluids.Select(f => CalculateForce(f, state))
                .Aggregate(Vector3.Zero, (prev, next) => prev + next);
            var position = GlobalTranslation;
            if (debugModeWasActive) DebugLineDrawer.ClearLinesStatic(this);
            if (DebugModeActive)
            {
                DebugLineDrawer.RegisterLineStatic(this, GlobalTranslation, GlobalTranslation + totalForce);
            }

            position -= target.GlobalTransform.origin;
            state.AddForce(totalForce, position);

            debugModeWasActive = DebugModeActive;
        }

        public static void SetDebugModeActive(bool newDebugModeActive)
        {
            DebugModeActive = newDebugModeActive;
            if (onDebugModeChanged != null) onDebugModeChanged.Invoke();
        }

        public override void _ExitTree()
        {
            if (Engine.EditorHint) return;

            target.UnregisterForcer(this);
            if (debugModeWasActive) DebugLineDrawer.ClearLinesStatic(this);
        }
    }
}