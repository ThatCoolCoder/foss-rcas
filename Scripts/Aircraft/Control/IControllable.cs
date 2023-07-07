using Godot;
using System;

namespace Aircraft.Control
{
    public interface IControllable
    {
        public IHub ControlHub { get; set; }
    }
}