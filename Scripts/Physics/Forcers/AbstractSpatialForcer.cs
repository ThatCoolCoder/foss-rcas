using Godot;
using System;

namespace Physics.Forcers
{
    public abstract partial class AbstractSpatialForcer : Node3D
    {
        // Base class for things that apply a force to a Forceable.
        // Note that this is tool-safe: extending classes can be [Tool]s without issues

        // Path to the target of this forcer. If parent is a SpatialForceable and path is null, then parent is used
        [Export] public NodePath TargetPath { get; set; }
        [Export] public bool Enabled { get; set; } = true;
        protected SpatialForceable target { get; private set; }

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            if (GetParent() is SpatialForceable t && (TargetPath == null || TargetPath == "")) target = t;
            else target = GetNode<SpatialForceable>(TargetPath);

            target.RegisterForcer(this);
        }

        public abstract void Apply(PhysicsDirectBodyState3D state);
    }
}