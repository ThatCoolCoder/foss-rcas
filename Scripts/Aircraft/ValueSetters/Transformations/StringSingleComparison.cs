using Godot;
using System;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class StringSingleComparison : AbstractValueTransformation
{
    [Export] public string OtherString { get; set; }
    [Export] public ComparisonMode Mode { get; set; } = ComparisonMode.Equal;

    public enum ComparisonMode
    {
        Equal,
        Contains,
        ContainedBy
    }

    public override dynamic Apply(dynamic value)
    {
        if (Mode == ComparisonMode.Equal) return value == OtherString;
        if (Mode == ComparisonMode.Contains) return value.Vontains(OtherString);
        if (Mode == ComparisonMode.ContainedBy) return OtherString.Contains(value);

        throw new Exception("This shouldn't happen");
    }
}