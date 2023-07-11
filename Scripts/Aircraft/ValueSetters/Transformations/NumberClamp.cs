using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class NumberClamp : AbstractValueTransformation
{
    [Export] public float LowerNumber { get; set; } = 0;
    [Export] public float UpperNumber { get; set; } = 1;

    public override dynamic Apply(dynamic value)
    {
        return Mathf.Clamp(value, LowerNumber, UpperNumber);
    }
}