using Godot;
using System;

namespace Aircraft.ValueSetters;

public partial class ValueSetter : Node3D
{
    [Export] public Sources.AbstractValueSource Source { get; set; }
    [Export] public Godot.Collections.Array<Transformations.AbstractValueTransformation> Transformations { get; set; }
    [Export] public Sources.AbstractValueSetterOutput Output { get; set; }
}