using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class NumberBetween : AbstractValueTransformation
{
    [Export] public float LowerNumber { get; set; } = 0;
    [Export] public float UpperNumber { get; set; } = 1;
    [Export] public bool Inclusive { get; set; } = true;

    public override dynamic Apply(dynamic value)
    {
        if (Inclusive) return LowerNumber <= value && value <= UpperNumber;
        else return LowerNumber < value && value < UpperNumber;
    }
}