using Godot;
using System;

namespace UI.Apps.Management;

[GlobalClass]
public partial class UIAppInfoList : Resource
{
    [Export] public Godot.Collections.Array<UIAppInfo> Apps { get; set; }
}