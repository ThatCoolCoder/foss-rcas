using Godot;
using System;

namespace SimSettings;

public partial class MiscSettings
{
    public string LastLoadedAircraft { get; set; } = null;
    public string LastLoadedLocation { get; set; } = null;
    public int PhysicsFps { get; set; } = 60 * 8; // might as well make this configurable since this might be a bit much for old computers, or computers where the GPU can't keep up
    public string AddonRepositoryPath { get; set; } = "user://AddonContent/";

    public void Apply()
    {
        Engine.PhysicsTicksPerSecond = PhysicsFps;
    }
}