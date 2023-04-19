using Godot;
using System;
using System.Collections.Generic;

namespace Locations
{
    public class CameraManager : Spatial
    {
        public static CameraManager instance;
        // Use of a hashset was considered for this, since we don't want duplicate cameras.
        // But we need preservation of order, so a list with manual checks in the accessors are used
        private List<IFlightCamera> cameras = new();
        private int activeCameraIndex = 0;
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
            if (index >= 0 && index < activeCameraIndex) activeCameraIndex--; // Prevent bad things happening when the index changes

            cameras.Remove(camera);
        }

        public void NextCamera()
        {
            ActivateCamera(activeCameraIndex + 1);
        }

        public void PreviousCamera()
        {
            ActivateCamera(activeCameraIndex - 1);
        }

        public void ActivateCamera(int newCameraIndex)
        {
            if (cameras.Count == 0) return;

            // Wrap to fit range
            while (newCameraIndex < 0) newCameraIndex += cameras.Count;
            while (newCameraIndex >= cameras.Count) newCameraIndex -= cameras.Count;

            // Handle activating
            cameras[newCameraIndex].Activate();
            if (newCameraIndex != activeCameraIndex && hasInitDefaultCamera) cameras[activeCameraIndex].Deactivate();
            activeCameraIndex = newCameraIndex;
        }

        public override void _Process(float delta)
        {
            if (!hasInitDefaultCamera)
            {
                hasInitDefaultCamera = true;
                ActivateCamera(0);
            }

            if (Input.IsActionJustPressed("prev_camera")) PreviousCamera();
            else if (Input.IsActionJustPressed("next_camera")) NextCamera();
        }
    }
}