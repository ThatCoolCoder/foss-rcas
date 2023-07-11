using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class NumberMap : AbstractValueTransformation
{
    [Export] public float OldMin { get; set; } = 0;
    [Export] public float OldMax { get; set; } = 1;
    [Export] public float NewMin { get; set; } = 0;
    [Export] public float NewMax { get; set; } = 1;
    [Export] public bool Clamp { get; set; } = false;

    public override dynamic Apply(dynamic value)
    {
        var newValue = Utils.MapNumber(value, OldMin, OldMax, NewMin, NewMax);
        if (Clamp) newValue = Mathf.Clamp(newValue, NewMin, NewMax);
        return newValue;
    }
}