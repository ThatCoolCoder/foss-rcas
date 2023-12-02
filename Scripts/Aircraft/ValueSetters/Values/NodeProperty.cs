using Godot;
using System;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public partial class NodeProperty : AbstractValue
{
    [Export] public NodePath NodePath { get; set; }
    [Export] public string Property { get; set; }

    protected override dynamic InternalGetValue(ValueSetter valueSetter)
    {
        return Utils.GetValueNested(valueSetter.GetNode(NodePath), Property);
    }

    protected override void InternalSetValue(dynamic value, ValueSetter valueSetter)
    {
        Utils.SetValueNested(valueSetter.GetNode(NodePath), Property, value);
    }
}