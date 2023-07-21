using Godot;
using System;

namespace UI.Apps.Management;

[GlobalClass]
public partial class UIAppInfo : Resource
{
    [Export] public string Name { get; set; }
    [Export] public Vector2 InitialSize { get; set; } = new Vector2(150, 300);
    [Export(PropertyHint.File, "*.tscn")] public string ScenePath { get; set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; set; }
}