using Godot;
using System;

namespace Aircraft
{
    public class OrbitCamera : Spatial, Locations.IFlightCamera
    {
        [Export] public float OrbitRadius { get; set; } = 1;
        [Export] public float Sensitivity { get; set; } = 1f;
        [Export] public string ViewName { get; set; } = "Orbit";
        private Vector2? clickStartPos = null;
        private float yRotation = 0;
        private float xRotation = 0;
        private float currentYRotation = 0;
        private float currentXRotation = 0;
        private Camera camera;

        public override void _Ready()
        {
            base._Ready();
            Locations.CameraManager.instance.AddCamera(this);
            camera = GetNode<Camera>("Camera");
            camera.Translation = new Vector3(0, 0, OrbitRadius);
        }

        public override void _ExitTree()
        {
            Locations.CameraManager.instance.RemoveCamera(this);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
        }

        public override void _Input(InputEvent _event)
        {
            if (!camera.Current) return;

            if (_event is InputEventMouseButton clickEvent)
            {
                if (clickEvent.Pressed)
                {
                    currentXRotation = 0;
                    currentYRotation = 0;
                    clickStartPos = clickEvent.Position;
                }
                else
                {
                    yRotation += currentYRotation;
                    xRotation += currentXRotation;
                    clickStartPos = null;
                }
            }
            else if (_event is InputEventMouseMotion motionEvent)
            {
                if (clickStartPos != null)
                {
                    var movement = motionEvent.Position - (Vector2)clickStartPos;
                    currentXRotation = -movement.y * Sensitivity * 0.001f;
                    currentYRotation = -movement.x * Sensitivity * 0.001f;

                    Rotation = new Vector3(xRotation + currentXRotation, yRotation + currentYRotation, 0);
                }
            }
        }

        public void Activate()
        {
            camera.Current = true;
        }

        public void Deactivate()
        {
            camera.Current = false;
        }
    }
}