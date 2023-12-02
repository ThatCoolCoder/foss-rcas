using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Tomlet.Attributes;

namespace ContentManagement;

public partial class Aircraft : ContentItem
{
    [PropName("wingspan")] public float WingSpan { get; set; }
    [PropName("length")] public float Length { get; set; }
    [PropName("weight")] public float Weight { get; set; }
    [PropName("power_type")] public AircraftPowerType PowerType { get; set; }
    [PropName("custom_power_type")] public string CustomPowerType { get; set; } = null;
    [PropName("channels")] public int ChannelCount { get; set; }
    [PropName("position_offset")] public Vector3 PositionOffset { get; set; } = Vector3.Zero;
    [PropName("config_properties")] public List<AircraftConfigProperty> ConfigProperties { get; set; } = new();
    [TomlNonSerialized] public bool NeedsLauncher { get; set; } = false;
    [PropName("launcher.height")] public float LauncherHeight { get; set; }
    [PropName("launcher.speed")] public float LauncherSpeed { get; set; }
    [PropName("launcher.angle")] public float LauncherAngleDegrees { get; set; }


    class PropNameAttribute : TomlPropertyAttribute
    {
        // shortcut for prepending the table name
        public PropNameAttribute(string mapFrom) : base("aircraft." + mapFrom)
        {
        }
    }

    protected override List<ContentProblem> InnerFindProblems()
    {
        var problems = new List<ContentProblem>();

        if (WingSpan == 0) problems.Add(new("Wingspan is zero"));
        if (Length == 0) problems.Add(new("Length is zero"));
        if (Weight == 0) problems.Add(new("Weight is zero"));

        var groupedByName = ConfigProperties.GroupBy(x => x.Name);
        if (groupedByName.Count() > 0 && groupedByName.Max(x => x.Count()) > 1)
        {
            var duplicatedNamesText = String.Join(", ", groupedByName.Where(x => x.Count() > 1).Select(x => x.Key));
            problems.Add(new($"There are multiple config properties with these internal names: {duplicatedNamesText}. Duplicates have been ignored"));

            foreach (var group in groupedByName.Where(x => x.Count() > 1))
            {
                // Delete all but first occurrence of name
                foreach (var item in group.Skip(1)) ConfigProperties.Remove(item);
            }
        }

        return problems;
    }
}