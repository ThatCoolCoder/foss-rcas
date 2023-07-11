using Godot;
using System;
using System.Linq;

namespace Aircraft.ValueSetters.Transformations;

[GlobalClass]
public partial class StringMultiComparison : AbstractValueTransformation
{
    [Export] public Godot.Collections.Array<string> OtherStrings { get; set; }
    [Export] public ComparisonMode Mode { get; set; } = ComparisonMode.EqualToAny;

    public enum ComparisonMode
    {
        EqualToAny,
        EqualToAll,
        ContainsAny,
        ContainsAll,
        ContainedByAny,
        ContainedByAll,
    }

    public override dynamic Apply(dynamic value)
    {
        if (Mode == ComparisonMode.EqualToAny) return OtherStrings.Any(x => value == x);
        if (Mode == ComparisonMode.EqualToAll) return OtherStrings.All(x => value == x);

        if (Mode == ComparisonMode.ContainsAny) return OtherStrings.Any(x => value.Contains(x));
        if (Mode == ComparisonMode.ContainsAll) return OtherStrings.All(x => value.Contains(x));

        if (Mode == ComparisonMode.ContainedByAny) return OtherStrings.Any(x => x.Contains(value));
        if (Mode == ComparisonMode.ContainedByAll) return OtherStrings.All(x => x.Contains(value));

        throw new Exception("This shouldn't happen");
    }
}