using Godot;
using System;
using System.Collections.Generic;

namespace Locations;

public partial class CameraManager : Node3D
{
    [Export] public Camera3D MapCamera { get; set; }
    public static CameraManager instance;
    public static readonly string UIMessageCategory = "camera";
    // Use of a hashset was considered for this, since we don't want duplicate cameras.
    // But we need preservation of order, so a list with manual checks in the accessors are used
    private List<IFlightCamera> cameras = new();
    public int ActiveCameraIndex { get; private set; } = 0;
    private bool hasInitDefaultCamera = false;

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _ExitTree()
    {
        instance = null;
    }

    public int AddCamera(IFlightCamera camera)
    {
        // Returns index of camera.
        if (cameras.Contains(camera)) return cameras.IndexOf(camera);
        else
        {
            cameras.Add(camera);
            camera.Deactivate();
            return cameras.Count - 1;
        }
    }

    public void RemoveCamera(IFlightCamera camera)
    {
        int index = cameras.IndexOf(camera);
        if (index >= 0 && index < ActiveCameraIndex) ActiveCameraIndex--; // Prevent bad things happening when the index changes

        cameras.Remove(camera);
    }

    public void NextCamera()
    {
        ActivateCamera(ActiveCameraIndex + 1);
    }

    public void PreviousCamera()
    {
        ActivateCamera(ActiveCameraIndex - 1);
    }

    public void ActivateCamera(int newCameraIndex)
    {
        if (cameras.Count == 0) return;

        // Wrap to fit range
        while (newCameraIndex < 0) newCameraIndex += cameras.Count;
        while (newCameraIndex >= cameras.Count) newCameraIndex -= cameras.Count;

        // Handle activating
        cameras[newCameraIndex].Activate();
        if (newCameraIndex != ActiveCameraIndex && hasInitDefaultCamera) cameras[ActiveCameraIndex].Deactivate();
        ActiveCameraIndex = newCameraIndex;

        UI.NotificationManager.AddNotification($"Camera: {cameras[newCameraIndex].ViewName}", UIMessageCategory);
    }

    public override void _Process(double delta)
    {
        if (!hasInitDefaultCamera)
        {
            hasInitDefaultCamera = true;
            ActivateCamera(0);
        }

        if (SimInput.Manager.IsActionJustPressed("gameplay/toggle_map") && MapCamera != null)
        {
            if (MapCamera.Current) ActivateCamera(ActiveCameraIndex); // reactivate normal camera
            else
            {
                if (hasInitDefaultCamera) cameras[ActiveCameraIndex].Deactivate();
                MapCamera.Current = true;
            }
        }
        else if (SimInput.Manager.IsActionJustPressed("camera/previous")) PreviousCamera();
        else if (SimInput.Manager.IsActionJustPressed("camera/next")) NextCamera();
    }
}