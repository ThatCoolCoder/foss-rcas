using Godot;
using System;
using System.Collections.Generic;
using Tomlet.Attributes;

namespace ContentManagement;

public partial class Location : ContentItem
{
    [PropName("world_location")] public string LocationInWorld { get; set; }
    [PropName("elevation")] public float Elevation { get; set; }
    [PropName("spawn_positions")] public List<AircraftSpawnPosition> SpawnPositions { get; set; } = new();

    class PropNameAttribute : TomlPropertyAttribute
    {
        // shortcut for prepending the table name
        public PropNameAttribute(string mapFrom) : base("location." + mapFrom)
        {
        }
    }

    protected override List<ContentProblem> InnerFindProblems()
    {
        var problems = new List<ContentProblem>();

        if (LocationInWorld.StripEdges().Length == 0) problems.Add(new("Location in world was not provided"));
        if (SpawnPositions.Count == 0) problems.Add(new("No spawn positions were listed", ProblemType.Error));

        return problems;
    }
}