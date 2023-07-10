using Godot;
using System;

namespace Locations
{
    public partial class GroundCamera : CharacterBody3D, IFlightCamera
    {
        [Export] public string ViewName { get; set; } = "Ground";
        [Export] public NodePath TargetPath { get; set; }

        [Export] public float MaxSpeed { get; set; } = 10;
        [Export] public float Acceleration { get; set; } = 40;
        [Export] public float JumpSpeed { get; set; } = 5;

        [Export] public float MaxAngularSpeedDegrees { get; set; } = 120;
        [Export] public float AngularAccelerationDegrees { get; set; } = 480;
        [Export] public float MouseSensitivity { get; set; } = 1;
        private ViewRotationManager rotationManager = new();


        public ZoomSettings CurrentZoomSettings;
        public Node3D Target { get; set; }

        private bool isWalkMode = false;
        private float startingFov;
        private Camera3D camera;

        public override void _Ready()
        {
            if (TargetPath != null) Target = GetNode<Node3D>(TargetPath);
            camera = GetNode<Camera3D>("Camera3D");

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

        public override void _Process(double delta)
        {
            if (!camera.Current) return;

            WalkMovement(delta);
            if (rotationManager.Update(delta)) StartWalkMode();

            if (SimInput.Manager.IsActionJustPressed("camera/reset"))
            {
                UI.MessageManager.StaticAddMessage("Ground camera: switched to tracking mode", CameraManager.UIMessageCategory);
                isWalkMode = false;
            }

            if (Target != null && !isWalkMode)
            {
                camera.LookAt(Target.GlobalPosition, Vector3.Up);

                if (CurrentZoomSettings.Enabled)
                {
                    var fovProportion = Mathf.Atan(1 / CurrentZoomSettings.StartDist) / Mathf.DegToRad(CurrentZoomSettings.BaseFov);

                    var distance = Target.GlobalPosition.DistanceTo(GlobalPosition);
                    var angle = Mathf.Atan(1 / (distance * CurrentZoomSettings.Factor)) / fovProportion;
                    angle = Mathf.RadToDeg(angle);
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
            rotationManager.SetPanAndTilt(camera.Rotation.X, camera.Rotation.Y);
            UI.MessageManager.StaticAddMessage("Ground camera: switched to walking mode", CameraManager.UIMessageCategory);
        }

        private void WalkMovement(double delta)
        {
            var fdelta = (float)delta;

            // A note on the way stuff is calculated:
            // to make damping and stuff dimensional, it's easy to work with local velocity.
            // So we just convert the velocity into local and then swap it back

            var localVelocity = Velocity.Rotated(Vector3.Up, -camera.GlobalRotation.Y);
            var origLocalVelocity = localVelocity;

            var crntAcceleration = new Vector3(SimInput.Manager.GetActionValue("camera/move_left_right"),
                0,
                -SimInput.Manager.GetActionValue("camera/move_backward_forward")) * Acceleration;

            localVelocity += crntAcceleration * fdelta;
            var _2dVel = new Vector2(localVelocity.X, localVelocity.Z).LimitLength(MaxSpeed);
            localVelocity = new Vector3(_2dVel.X, localVelocity.Y, _2dVel.Y);

            if (crntAcceleration.X == 0) localVelocity.X = Utils.ConvergeValue(localVelocity.X, 0, Acceleration * fdelta);
            if (crntAcceleration.Z == 0) localVelocity.Z = Utils.ConvergeValue(localVelocity.Z, 0, Acceleration * fdelta);
            var jumped = false;
            if (SimInput.Manager.IsActionJustPressed("camera/move_down_up"))
            {
                localVelocity.Y = JumpSpeed;
                jumped = true;
            }

            if (crntAcceleration.LengthSquared() > 0 || jumped) StartWalkMode();

            localVelocity.Y -= 9.8f * fdelta;

            Velocity += (localVelocity - origLocalVelocity).Rotated(Vector3.Up, camera.GlobalRotation.Y);

            MoveAndSlide();
        }

        public override void _UnhandledInput(InputEvent _event)
        {
            if (camera.Current)
            {
                if (rotationManager.UnhandledInput(_event)) StartWalkMode();
            }
        }

        public partial class ZoomSettings
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
