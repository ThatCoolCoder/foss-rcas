using Godot;
using System;
using System.Collections.Generic;

namespace Aircraft.Control;

public partial class ChannelMixSet
{
    public Dictionary<string, float> CustomDefaultValues { get; set; } = new();
    public List<ChannelMix> Mixes { get; set; } = new();
}