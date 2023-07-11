using Godot;
using System;

namespace Aircraft.ValueSetters.Sources;

[GlobalClass]
public partial class AircraftConfigSource : AbstractValueSource
{
    [Export] public Aircraft Aircraft { get; set; }
    [Export] public string Property { get; set; }

    public override dynamic GetValue()
    {
        // GD.Print(Tomlet.TomletMain.TomlStringFrom(ACConfig));
        return Aircraft.Config[Property];
    }
}