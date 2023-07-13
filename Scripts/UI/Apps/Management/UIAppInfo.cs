using Godot;
using System;

namespace UI.Apps.Management;

[GlobalClass]
public partial class UIAppInfo : Resource
{
    [Export] public string Name { get; set; }
    [Export(PropertyHint.File, "*.tscn")] public string ScenePath { get; set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; set; }
}