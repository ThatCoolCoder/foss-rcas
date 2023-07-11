using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class StringLength : AbstractValueTransformation
{
    public override dynamic Apply(dynamic value)
    {
        return (value as string).Length;
    }
}