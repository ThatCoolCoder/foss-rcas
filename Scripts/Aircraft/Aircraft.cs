using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aircraft;

public partial class Aircraft : Physics.SpatialForceable
{
    public Dictionary<string, dynamic> Config { get; set; } = new();

    public void Reset()
    {
        PropagateCall("OnAircraftReset");
    }
}