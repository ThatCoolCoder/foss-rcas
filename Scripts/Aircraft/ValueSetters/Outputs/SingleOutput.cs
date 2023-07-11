using Godot;
using System;

namespace Aircraft.ValueSetters.Sources;

[GlobalClass]
public partial class SingleOutput : AbstractValueSetterOutput
{
    [Export] public Node Node { get; set; }
    [Export] public string Property { get; set; }

    public override void Apply(dynamic value)
    {
        Node.Set(Property, value);
    }
}