using System;
using System.Collections.Generic;
using Godot;
using System.Linq;
using Tomlet;

namespace Misc;

public static class TomletSubclassMapper
{
    // Thing that allows storing polymorphic objects in toml by storing the class name in there.
    // Uses list of permitted classes to avoid vulnerabilities.
    // Automatically allows loading the base class if it's not abstract or interface so you don't need to put that in allowableLoadedTypes
    // Sadly it's not possible to get this working with types where TBase is generic and you want to deserialise an X where X : TBase<string> or whatever
    public static void CreateMapping<TBase>(Type[] allowableLoadedTypes, string typeNameField = "__TomlTypeName") where TBase : class
    {
        var baseType = typeof(TBase);

        // These are internal so we have to be a bit cheeky to grab them
        var tomlCompositeSerializer = typeof(TomletMain).Assembly.GetType("Tomlet.TomlCompositeSerializer");
        var tomlCompositeDeserializer = typeof(TomletMain).Assembly.GetType("Tomlet.TomlCompositeDeserializer");
        var registerMapper = typeof(TomletMain).GetMethod("RegisterMapper", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

        var internalDeserializers = new Dictionary<string, Tomlet.TomlSerializationMethods.Deserialize<object>>();

        // Register serializers for all the derived types & base as well
        if (!baseType.IsInterface && !baseType.IsAbstract) allowableLoadedTypes = allowableLoadedTypes.Append(baseType).ToArray();
        foreach (var subType in allowableLoadedTypes)
        {
            if (!subType.IsSubclassOf(baseType) && baseType != subType) throw new Exception($"Allowable loaded type {subType.Name} is not a subclass of {baseType.Name}");


            var serializerInternal = (Tomlet.TomlSerializationMethods.Serialize<TBase>)tomlCompositeSerializer.GetMethod("For").Invoke(null, new object[] { subType });
            var deserializerInternal = (Tomlet.TomlSerializationMethods.Deserialize<object>)tomlCompositeDeserializer.GetMethod("For").Invoke(null, new object[] { subType });
            internalDeserializers[subType.Name] = deserializerInternal;

            Tomlet.TomlSerializationMethods.Serialize<TBase> serializer = (TBase baseInstance) =>
            {
                var table = (Tomlet.Models.TomlTable)serializerInternal(baseInstance);
                table.Put(typeNameField, baseInstance.GetType().Name);
                return table;
            };

            registerMapper
                .MakeGenericMethod(subType)
                .Invoke(null, parameters: new object[] { serializer, null });
        }

        // Register deserializer for the base type
        var baseDeserializerInternal = (Tomlet.TomlSerializationMethods.Deserialize<object>)tomlCompositeDeserializer.GetMethod("For").Invoke(null, new object[] { typeof(TBase) });
        TomletMain.RegisterMapper<TBase>(
            null,
            tomlValue =>
            {
                if (!(tomlValue is Tomlet.Models.TomlTable tomlTable))
                    throw new Tomlet.Exceptions.TomlTypeMismatchException(typeof(Tomlet.Models.TomlTable), tomlValue.GetType(), typeof(TBase));

                if (!tomlTable.ContainsKey(typeNameField)) throw new Exception("Error loading {baseType.Name}: TypeName not given!");
                var typeName = tomlTable.GetString(typeNameField);

                var cls = allowableLoadedTypes.FirstOrDefault(cls => cls.Name == typeName);
                if (cls == null) throw new Exception($"Error loading {baseType.Name}: Not allowed to parse a {typeName}");

                return (TBase)internalDeserializers[typeName](tomlValue);
            }
        );
    }
}