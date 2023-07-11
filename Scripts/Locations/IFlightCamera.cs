using Godot;
using System;

namespace Locations;

public interface IFlightCamera
{
    // Camera used for flying the plane - EG on the ground or FPV

    public string ViewName { get; set; }
    public void Activate();
    public void Deactivate();
}