using Godot;
using System;
using System.Reflection;
using System.Linq;

namespace Aircraft.ValueSetters.Values;

[GlobalClass]
public abstract partial class AbstractValue : Resource
{
    protected abstract dynamic InternalGetValue(ValueSetter valueSetter);
    protected abstract void InternalSetValue(dynamic value, ValueSetter valueSetter);
    public dynamic GetValue(ValueSetter valueSetter)
    {
        var result = InternalGetValue(valueSetter);
        Type t = result.GetType();
        if (!permittedTypes.Contains(t)) throw new Exceptions.UnknownTypeException(t);
        return result;
    }
    public void SetValue(dynamic value, ValueSetter valueSetter)
    {
        Type t = value.GetType();
        if (!permittedTypes.Contains(t)) throw new Exceptions.UnknownTypeException(t);

        InternalSetValue(value, valueSetter);
    }

    private readonly Type[] permittedTypes = new Type[]
    {
        typeof(float),
        typeof(int),
        typeof(string),
    };
}