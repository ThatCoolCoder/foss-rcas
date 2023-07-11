using Godot;
using System;

namespace Aircraft.ValueSetters.Sources;

[GlobalClass]
public abstract partial class AbstractValueSource : Resource
{
    public abstract dynamic GetValue();
}