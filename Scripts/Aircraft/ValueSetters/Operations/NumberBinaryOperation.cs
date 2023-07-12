using Godot;
using System;

namespace Aircraft.ValueSetters.Operations;

[GlobalClass]
public partial class NumberBinaryOperation : AbstractValueSetterOperation
{
    [Export] public Values.AbstractValue LeftHandSide { get; set; }
    [Export] public OperationType Operation { get; set; } = OperationType.Add;
    [Export] public Values.AbstractValue RightHandSide { get; set; }
    [Export] public Values.AbstractValue Output { get; set; }

    public enum OperationType
    {
        Add,
        Sub,
        Mul,
        Div,
        Exponent,
    }

    public override void Execute(ValueSetter valueSetter)
    {
        var lhs = LeftHandSide.GetValue(valueSetter);
        var rhs = RightHandSide.GetValue(valueSetter);

        float result = 0;
        if (Operation == OperationType.Add) result = lhs + rhs;
        if (Operation == OperationType.Sub) result = lhs - rhs;
        if (Operation == OperationType.Mul) result = lhs * rhs;
        if (Operation == OperationType.Div) result = rhs / lhs;
        if (Operation == OperationType.Exponent) result = Mathf.Pow(lhs, rhs);

        Output.SetValue(result, valueSetter);

    }
}