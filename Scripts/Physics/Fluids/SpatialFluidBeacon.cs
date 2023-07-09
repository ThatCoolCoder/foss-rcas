using Godot;
using System;
using System.Linq;

namespace Physics.Fluids
{
    // Class that registers a ISpatialFluid (normally its parent, but can have a custom node path) in a SpatialFluidRepository
    public partial class SpatialFluidBeacon : Node3D
    {
        [Export] public NodePath FluidPath { get; set; } = null;
        [Export] public bool Enabled { get; set; } = true;

        public override void _Ready()
        {
            if (!Enabled) return;

            var repository = SpatialFluidRepository.FindSpatialFluidRepository(GetTree());
            if (GetParent() is ISpatialFluid f)
            {
                repository.AddFluid(f);
            }
            else if (FluidPath != null)
            {
                repository.AddFluid(GetNode<ISpatialFluid>(FluidPath));
            }
            else
            {
                Utils.LogError($"SpatialFluidBeacon is not a child of an ISpatialFluid and has no FluidPath defined", this);
            }
        }
    }
}