using Godot;
using System;

namespace Aircraft.Control
{
    public interface IControllable
    {
        public Hub ControlHub { get; set; }
    }
}