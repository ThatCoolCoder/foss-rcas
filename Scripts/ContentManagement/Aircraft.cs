using Godot;
using System;
using Tomlet.Attributes;

namespace ContentManagement
{
    public class Aircraft : ContentItem
    {
        [PropName("wingspan")] public float WingSpan { get; set; }
        [PropName("length")] public float Length { get; set; }
        [PropName("weight")] public float Weight { get; set; }
        [PropName("power_type")] public AircraftPowerType PowerType { get; set; }
        [PropName("custom_power_type")] public string CustomPowerType { get; set; } = null;
        [PropName("channels")] public int ChannelCount { get; set; }
        [PropName("needs_launcher")] public bool NeedsLauncher { get; set; } = false;
        [PropName("launcher_height")] public float LauncherHeight { get; set; }
        [PropName("launcher_speed")] public float LauncherSpeed { get; set; }

        class PropNameAttribute : TomlPropertyAttribute
        {
            // shortcut for prepending the table name
            public PropNameAttribute(string mapFrom) : base("aircraft." + mapFrom)
            {
            }
        }
    }
}