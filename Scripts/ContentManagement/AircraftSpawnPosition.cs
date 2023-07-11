using Godot;
using System;
using Tomlet.Attributes;

namespace ContentManagement;

public partial class AircraftSpawnPosition
{
    [TomlPropertyAttribute("name")] public string Name { get; set; } = "Position 1";
    [TomlPropertyAttribute("aircraft_at")] public string AircraftPositionNodePath { get; set; } = "Positions/SpawnPosition1";
    [TomlPropertyAttribute("camera_at")] public string CameraPositionNodePath { get; set; } = "Positions/CameraPosition1";
}