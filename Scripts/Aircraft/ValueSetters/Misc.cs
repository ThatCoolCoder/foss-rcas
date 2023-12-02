using Godot;
using System;

namespace Aircraft.ValueSetters;

class Misc
{
    public static T TryCast<T>(dynamic value)
    {
        try
        {
            return (T)value;
        }
        catch (Exception)
        {
            throw new Exceptions.InvalidTypeException(value.GetType(), "");
        }
    }
}