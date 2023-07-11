using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class NumberSqrt : AbstractValueTransformation
{
    public override dynamic Apply(dynamic value)
    {
        return Mathf.Sqrt(value);
    }
}