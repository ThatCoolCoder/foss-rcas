using Godot;
using System;

namespace Locations;

public partial class BasicFlightCamera : Camera3D, IFlightCamera
{
    // Camera used for flying the plane - EG on the ground or FPV

    [Export] public string ViewName { get; set; } = "Unnamed";
    [Export] public bool UseDefaultFov { get; set; } = true;

    public override void _Ready()
    {
        CameraManager.instance.AddCamera(this);
        if (UseDefaultFov) Fov = SimSettings.Settings.Current.View.Fov;
    }

    public override void _ExitTree()
    {
        CameraManager.instance.RemoveCamera(this);
    }

    public void Activate()
    {
        BeforeActivated();
        Current = true;
        OnActivated();
    }
    public virtual void BeforeActivated() { }
    public virtual void OnActivated() { }


    public void Deactivate()
    {
        BeforeDeactivated();
        Current = false;
        OnDeactivated();
    }
    public virtual void BeforeDeactivated() { }
    public virtual void OnDeactivated() { }
}