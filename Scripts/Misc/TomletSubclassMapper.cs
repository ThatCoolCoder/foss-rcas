using System;
using Godot;
using System.Linq;
using Tomlet;

namespace Misc;

public static class TomletSubclassMapper
{

    // Thing that allows storing polymorphic objects in toml by storing the class name in there.
    // Uses list of permitted classes to avoid vulnerabilities.
    public static void CreateMapping<TBase>(Type[] allowableLoadedTypes, string typeNameField = "__TomlTypeName") where TBase : class
    {
        foreach (var subType in allowableLoadedTypes)
        {
            if (!subType.IsSubclassOf(typeof(TBase))) throw new Exception($"Allowable loaded type {subType.Name} is not a subclass of {typeof(TBase).Name}");

            var method = typeof(TomletMain).GetMethod("RegisterMapper", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            Tomlet.TomlSerializationMethods.Serialize<TBase> converter = (TBase st) =>
            {
                GD.Print(st.GetType().Name);
                // throw new Exception("don't know what to return");
                return Tomlet.Models.TomlBoolean.True;
            };

            method
                .MakeGenericMethod(subType)
                .Invoke(null, parameters: new object[] { converter, null });
            GD.Print(subType.Name);
        }

        TomletMain.RegisterMapper<TBase>(
            tBase =>
            {
                GD.Print(tBase.GetType().Name);
                throw new Exception("don't know what to return");
            },
            tomlValue =>
            {
                if (!(tomlValue is Tomlet.Models.TomlTable tomlTable))
                    throw new Tomlet.Exceptions.TomlTypeMismatchException(typeof(Tomlet.Models.TomlTable), tomlValue.GetType(), typeof(TBase));

                var typeName = tomlTable.GetString(typeNameField);
                if (typeName == null) throw new Exception("Error loading control mapping: TypeName not given!");

                var cls = allowableLoadedTypes.FirstOrDefault(cls => cls.Name == typeName);
                if (cls == null) throw new Exception($"Error loading control mapping: Not allowed to parse a {typeName}");

                // Use reflection to call the generic method using the type we found before 
                // Have fun when this inevitably breaks
                return typeof(TomletMain)
                    .GetMethod("To", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                    null, System.Reflection.CallingConventions.Any,
                    new Type[] { typeof(Tomlet.Models.TomlValue) }, null)
                    .MakeGenericMethod(cls)
                    .Invoke(null, parameters: new object[] { tomlTable }) as TBase;
            }
        );
    }
}