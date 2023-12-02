using Godot;
using System;
using System.Linq;

namespace Aircraft.ValueSetters.Exceptions;

public class ValueSetterException : Exception
{
    public ValueSetterException(string message, Exception inner = null) : base("Error in value setter: " + message, inner)
    {

    }

    public string OperationName { get; set; } = "";
    public int OperationNumber { get; set; }
}

public class CannotSetException : ValueSetterException
{
    public CannotSetException(string triedSettingWhat) : base($"Cannot set {triedSettingWhat} - it is read only")
    {

    }
}

public class UnknownTypeException : ValueSetterException
{
    public UnknownTypeException(Type type) : base($"type \"{type.Name}\" is not permitted in value setters")
    {

    }
}

public class InvalidTypeException : ValueSetterException
{
    public InvalidTypeException(Type type, string context) : base($"a(n) {type.Name} is not valid in {context}")
    {

    }
}

public class IncompatibleTypesException : ValueSetterException
{
    public IncompatibleTypesException(Type[] types, string context) : base($"the following types are not compatible in {context}: {GenerateTypeList(types)}")
    {

    }

    private static string GenerateTypeList(Type[] types)
    {
        return String.Join(", ", types.Select(x => x.Name));
    }
}