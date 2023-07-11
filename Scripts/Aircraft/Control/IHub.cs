using Godot;
using System;
using System.Collections.Generic;


namespace Aircraft.Control;

public interface IHub
{
    public Dictionary<string, float> ChannelValues { get; set; }
}