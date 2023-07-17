using Godot;
using System;

namespace SimSettings;

public class ViewSettings
{
    public float Fov
    {
        get { return GroundCameraZoom.BaseFov; }
        set { GroundCameraZoom.BaseFov = value; }
    }
    public Locations.GroundCamera.ZoomSettings GroundCameraZoom { get; set; } = new();
    public bool AdjustZoomForAircraftSize { get; set; } = true;
}