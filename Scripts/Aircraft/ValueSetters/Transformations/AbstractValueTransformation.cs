using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public abstract partial class AbstractValueTransformation : Resource
{
    public abstract dynamic Apply(dynamic value);
}