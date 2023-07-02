using Godot;
using System;

namespace Locations
{
    public class GroundCamera : KinematicBody, IFlightCamera
    {
        [Export] public string ViewName { get; set; } = "Ground";
        [Export] public NodePath TargetPath { get; set; }

        private Vector3 localVelocity; // (in local space)
        [Export] public float MaxSpeed { get; set; } = 10;
        [Export] public float Acceleration { get; set; } = 40;
        [Export] public float JumpSpeed { get; set; } = 5;

        [Export] public float MaxAngularSpeedDegrees { get; set; } = 120;
        [Export] public float AngularAccelerationDegrees { get; set; } = 480;
        [Export] public float MouseSensitivity { get; set; } = 1;
        private ViewRotationManager rotationManager = new();


        public ZoomSettings CurrentZoomSettings;
        public Spatial Target { get; set; }

        private bool isWalkMode = false;
        private float startingFov;
        private Camera camera;

        public override void _Ready()
        {
            if (TargetPath != null) Target = GetNode<Spatial>(TargetPath);
            camera = GetNode<Camera>("Camera");

            rotationManager.Target = camera;
            rotationManager.MaxAngularSpeedDegrees = MaxAngularSpeedDegrees;
            rotationManager.AngularAccelerationDegrees = AngularAccelerationDegrees;
            rotationManager.MouseSensitivity = MouseSensitivity;

            startingFov = camera.Fov;
            CameraManager.instance.AddCamera(this);

            base._Ready();

        }

        public override void _ExitTree()
        {
            CameraManager.instance.RemoveCamera(this);
        }

        public void Activate()
        {
            camera.Current = true;
        }

        public void Deactivate()
        {
            camera.Current = false;
        }

        public override void _Process(float delta)
        {
            if (!camera.Current) return;

            WalkMovement(delta);
            if (rotationManager.Update(delta)) StartWalkMode();

            if (SimInput.Manager.IsActionJustPressed("camera/reset"))
            {
                UI.MessageManager.StaticAddMessage("Ground camera: switched to tracking mode");
                isWalkMode = false;
            }

            if (Target != null && !isWalkMode)
            {
                camera.LookAt(Target.GlobalTranslation, Vector3.Up);

                if (CurrentZoomSettings.Enabled)
                {
                    var fovProportion = Mathf.Atan(1 / CurrentZoomSettings.StartDist) / Mathf.Deg2Rad(CurrentZoomSettings.BaseFov);

                    var distance = Target.GlobalTranslation.DistanceTo(GlobalTranslation);
                    var angle = Mathf.Atan(1 / (distance * CurrentZoomSettings.Factor)) / fovProportion;
                    angle = Mathf.Rad2Deg(angle);
                    angle = Mathf.Clamp(angle, 1, CurrentZoomSettings.BaseFov);

                    camera.Fov = angle;
                }
            }
        }

        private void StartWalkMode()
        {
            if (isWalkMode) return;

            isWalkMode = true;
            camera.Fov = startingFov;
            rotationManager.SetPanAndTilt(camera.Rotation.x, camera.Rotation.y);
            UI.MessageManager.StaticAddMessage("Ground camera: switched to walking mode");
        }

        private void WalkMovement(float delta)
        {
            var crntAcceleration = new Vector3(SimInput.Manager.GetActionValue("camera/move_left_right"),
                0,
                -SimInput.Manager.GetActionValue("camera/move_backward_forward")) * Acceleration;

            var origLocalVelocity = localVelocity;

            localVelocity += crntAcceleration * delta;
            var _2dVel = new Vector2(localVelocity.x, localVelocity.z).LimitLength(MaxSpeed);
            localVelocity = new Vector3(_2dVel.x, localVelocity.y, _2dVel.y);

            if (crntAcceleration.x == 0) localVelocity.x = Utils.ConvergeValue(localVelocity.x, 0, Acceleration * delta);
            if (crntAcceleration.z == 0) localVelocity.z = Utils.ConvergeValue(localVelocity.z, 0, Acceleration * delta);
            if (SimInput.Manager.IsActionJustPressed("camera/move_down_up")) localVelocity.y = JumpSpeed;

            if (origLocalVelocity != localVelocity) StartWalkMode();

            localVelocity.y -= 9.8f * delta;

            var velocity = MoveAndSlide(localVelocity.Rotated(Vector3.Up, camera.GlobalRotation.y));
            localVelocity = velocity.Rotated(Vector3.Up, -camera.GlobalRotation.y);
        }

        public override void _Input(InputEvent _event)
        {
            if (camera.Current)
            {
                if (rotationManager.Input(_event)) StartWalkMode();
            }
        }

        public class ZoomSettings
        {
            public bool Enabled { get; set; } = true;
            public float BaseFov { get; set; } = 70;
            public float StartDist { get; set; } = 25; // Maximum distance that plane can still be seen with base FOV

            // Rate of zoom compared to the "perfect rate". If this wasn't present, it would zoom perfectly and keep the plane the same size forever.
            // However, that would make judging distance difficult, so by setting this to something less than 1, it gives the best of both worlds
            public float Factor { get; set; } = 0.7f;
        }
    }
}
