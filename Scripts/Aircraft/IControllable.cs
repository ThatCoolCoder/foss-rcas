using Godot;
using System;

namespace Aircraft
{
    public interface IControllable
    {
        public ControlHub ControlHub { get; set; }
    }
}