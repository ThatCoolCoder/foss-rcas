using Godot;
using System;

namespace Aircraft.ValueSetters.Sources;

[GlobalClass]
public partial class AircraftConfigSource : AbstractValueSource
{
    [Export] public RigidBody3D Aircraft { get; set; }

    public override dynamic GetValue()
    {
        return 42;
    }
}