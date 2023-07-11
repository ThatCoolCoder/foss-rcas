using Godot;
using System;
using System.Collections.Generic;

namespace Physics.Fluids;

public partial class SpatialFluidRepository : Node3D
{

    // Please do not edit this property directly, use methods of the class
    public List<ISpatialFluid> Fluids { get; private set; } = new();

    public static string GroupName = "SpatialFluidRepository";

    public override void _Ready()
    {
        AddToGroup(GroupName);
    }

    public void AddFluid(ISpatialFluid fluid)
    {
        Fluids.Add(fluid);
    }

    public void RemoveFluid(ISpatialFluid fluid)
    {
        Fluids.Remove(fluid);
    }

    public static SpatialFluidRepository FindSpatialFluidRepository(SceneTree tree)
    {
        var nodes = tree.GetNodesInGroup(SpatialFluidRepository.GroupName);
        var repository = nodes.Count > 0 ? nodes[0] as SpatialFluidRepository : null;
        if (repository == null)
        {
            Utils.LogError($"Unable to find a SpatialFluidRepository in the scene");
        }
        return repository;
    }
}