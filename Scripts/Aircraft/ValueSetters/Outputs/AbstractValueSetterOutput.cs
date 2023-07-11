using Godot;
using System;

namespace Aircraft.ValueSetters.Sources
{
    [GlobalClass]
    public abstract partial class AbstractValueSetterOutput : Resource
    {
        public abstract void Apply(dynamic value);
    }
}