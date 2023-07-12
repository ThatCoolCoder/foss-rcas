using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class NumberBasicComparison : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue LeftHandSide { get; set; }
    [Export] public Values.AbstractValue RightHandSide { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }

    [Export] public ComparisonType Operation { get; set; } = ComparisonType.Equal;

    public enum ComparisonType
    {
        Equal,
        GreaterThan,
        GreaterThanEqual,
        LesserThan,
        LesserThanEqual
    }

    public override void Execute(ValueSetter valueSetter)
    {
        var lhs = LeftHandSide.GetValue(valueSetter);
        var rhs = RightHandSide.GetValue(valueSetter);

        float result = 0;

        if (Operation == ComparisonType.Equal) result = lhs == rhs;
        if (Operation == ComparisonType.GreaterThan) result = lhs > rhs;
        if (Operation == ComparisonType.GreaterThanEqual) result = lhs >= rhs;
        if (Operation == ComparisonType.LesserThan) result = lhs < rhs;
        if (Operation == ComparisonType.LesserThanEqual) result = lhs <= rhs;

        Output.SetValue(Convert.ToInt32(result), valueSetter);
    }
}