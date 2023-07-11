using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class BoolNot : AbstractValueTransformation
{
    public override dynamic Apply(dynamic value)
    {
        return !value;
    }
}