using Godot;
using System;
using Tomlet.Attributes;

namespace ContentManagement
{
    public class Location : ContentItem
    {
        [PropName("world_location")] public string LocationInWorld { get; set; }
        [PropName("elevation")] public float Elevation { get; set; }

        class PropNameAttribute : TomlPropertyAttribute
        {
            // shortcut for prepending the table name
            public PropNameAttribute(string mapFrom) : base("location." + mapFrom)
            {
            }
        }
    }
}