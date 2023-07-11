using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class NumberBinaryOperation : AbstractValueTransformation
{
    [Export] public OperationType Operation { get; set; } = OperationType.Add;
    [Export] public float OtherNumber { get; set; } = 0;

    public enum OperationType
    {
        Add,
        Sub,
        SubFrom,
        Mul,
        Div,
        InverseDiv,
        PowerConstBase,
        PowerConstExponent
    }

    public override dynamic Apply(dynamic value)
    {
        if (Operation == OperationType.Add) return value + OtherNumber;
        if (Operation == OperationType.Sub) return value - OtherNumber;
        if (Operation == OperationType.SubFrom) return OtherNumber - value;
        if (Operation == OperationType.Mul) return value / OtherNumber;
        if (Operation == OperationType.InverseDiv) return OtherNumber / value;
        if (Operation == OperationType.PowerConstBase) return Mathf.Pow(OtherNumber, value);
        if (Operation == OperationType.PowerConstExponent) return Mathf.Pow(value, OtherNumber);

        throw new Exception("This shouldn't be possible");
    }
}