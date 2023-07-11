using Godot;
using System;

namespace Aircraft.ValueSetters.Sources;

[GlobalClass]
public partial class AircraftConfigSource : AbstractValueSource
{
    [Export] public NodePath AircraftPath { get; set; }
    [Export] public string Property { get; set; }

    public override dynamic GetValue(Node valueSetter)
    {
        return valueSetter.GetNode<Aircraft>(AircraftPath).Config[Property];
    }
}