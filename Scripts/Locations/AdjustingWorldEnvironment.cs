using Godot;
using System;

namespace Locations;

public partial class AdjustingWorldEnvironment : WorldEnvironment
{
    // Need a better name - thing that sets certain properties to the world environment from the game settings

    public override void _Ready()
    {
        var g = SimSettings.Settings.Current.Graphics;
        Environment.SdfgiEnabled = g.GlobalIllumination;
        Environment.SsilEnabled = g.IndirectLighting;
        Environment.SsaoEnabled = g.AmbientOcclusion;
        Environment.SsrEnabled = g.Reflections;
    }
}