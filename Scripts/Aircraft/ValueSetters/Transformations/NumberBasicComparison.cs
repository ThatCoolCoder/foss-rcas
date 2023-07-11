using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class NumberBasicComparison : AbstractValueTransformation
{
    [Export] public ComparisonType Operation { get; set; } = ComparisonType.Equal;
    [Export] public float OtherNumber { get; set; } = 0;

    public enum ComparisonType
    {
        Equal,
        GreaterThan,
        GreaterThanEqual,
        LesserThan,
        LesserThanEqual
    }

    public override dynamic Apply(dynamic value)
    {
        if (Operation == ComparisonType.Equal) return value == OtherNumber;
        if (Operation == ComparisonType.GreaterThan) return value > OtherNumber;
        if (Operation == ComparisonType.GreaterThanEqual) return value >= OtherNumber;
        if (Operation == ComparisonType.LesserThan) return value < OtherNumber;
        if (Operation == ComparisonType.LesserThanEqual) return value <= OtherNumber;

        throw new Exception("This shouldn't be possible");
    }
}