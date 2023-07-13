using Godot;
using System;
using System.Collections.Generic;

namespace UI.Apps.Management;

public class AppProfile
{
    public string Name { get; set; } = "Unnamed profile";
    public bool IsDefault { get; set; } = false;
    public List<AppLayoutInfo> Apps { get; set; } = new();
}

public class AppLayoutInfo
{
    public string ScenePath { get; set; }
    // public Vector2
}