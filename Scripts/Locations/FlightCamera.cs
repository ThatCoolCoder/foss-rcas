using Godot;
using System;

namespace Locations
{
    public class FlightCamera : Camera
    {
        // Camera used for flying the plane - EG on the ground or FPV

        [Export] public string ViewName { get; set; } = "Unnamed"; // will eventually be used when notifying of changed camera. todo: remove comment when that is done

        public override void _Ready()
        {
            CameraManager.instance.AddCamera(this);
        }

        public override void _ExitTree()
        {
            CameraManager.instance.RemoveCamera(this);
        }
    }
}