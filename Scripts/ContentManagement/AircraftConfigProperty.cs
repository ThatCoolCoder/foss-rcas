using System;
using System.Collections.Generic;

namespace ContentManagement;

public abstract class AircraftConfigProperty
{
    public static void CreateSubclassMappers()
    {
        Misc.TomletSubclassMapper.CreateMapping<AircraftConfigProperty>(
            new Type[] {
                typeof(String),
                typeof(Boolean),
                typeof(Slider),
                typeof(SpinBox),
                typeof(Enum),
            }, "PropertyType");
    }

    public string DisplayName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    // Would love to have used generics to have an abstract T DefaultValue but it just wasn't playing nicely with the subclass mapper

    public class String : AircraftConfigProperty
    {
        // Yes we could use generics instead of making all of these define their own default
        // but that would probably make the subclass mapper have a literal heart attack
        // and it's not that important because we branch based on type before using the defaults anyway
        public string DefaultValue { get; set; } = "";
    }

    public class Boolean : AircraftConfigProperty
    {
        public bool DefaultValue { get; set; } = true;
    }

    public abstract class Number : AircraftConfigProperty
    {
        // Abstract purely so that we can't instantiate it without specifying type
        public float Min { get; set; } = 0;
        public float Max { get; set; } = 10;
        public float Step { get; set; } = 0.1f;
        public float DefaultValue { get; set; } = 0;
    }

    public class Slider : Number
    {
        public int DecimalsDisplayed { get; set; } = 1;
    }

    public class SpinBox : Number
    {
        public float ArrowStep { get; set; } = -1;
    }

    public class Enum : AircraftConfigProperty
    {
        public List<string> PossibleValues { get; set; } = new();
        public string DefaultValue { get; set; } = "";
    }
}