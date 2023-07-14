using Godot;
using System;
using System.Collections.Generic;

namespace UI.Apps.Management;

public class AppProfile
{
    public string Name { get; set; } = "Unnamed profile";
    public bool IsDefault { get; set; } = false;
    public List<AppLayoutInfo> Apps { get; set; } = new();

    // todo: have a sensible default
    public static AppProfile Default = new()
    {
        Name = "Default",
        IsDefault = true,
        Apps = { }
    };
}

public class AppLayoutInfo
{
    public string ScenePath { get; set; }

    public float AnchorLeft { get; set; }
    public float AnchorRight { get; set; }
    public float AnchorTop { get; set; }
    public float AnchorBottom { get; set; }

    // tomlet really doesn't appreciate godot vectors so after 1 1/2 hours I declare that this is good enough
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float SizeX { get; set; }
    public float SizeY { get; set; }
}