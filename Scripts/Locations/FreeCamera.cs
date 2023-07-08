using Godot;
using System;

namespace Locations
{
    public class FreeCamera : BasicFlightCamera
    {
        private Vector3 velocity; // (in local space)
        [Export] public float MaxSpeed { get; set; } = 10;
        [Export] public float Acceleration { get; set; } = 40;

        [Export] public float MaxAngularSpeedDegrees { get; set; } = 120;
        [Export] public float AngularAccelerationDegrees { get; set; } = 480;
        [Export] public float MouseSensitivity { get; set; } = 1;

        private ViewRotationManager rotationManager = new();

        public override void _Ready()
        {
            rotationManager.Target = this;
            rotationManager.MaxAngularSpeedDegrees = MaxAngularSpeedDegrees;
            rotationManager.AngularAccelerationDegrees = AngularAccelerationDegrees;
            rotationManager.MouseSensitivity = MouseSensitivity;

            base._Ready();
        }

        public override void _Process(float delta)
        {
            if (Current)
            {
                KeyboardMovement(delta);

                rotationManager.Update(delta);
            }
        }

        private void KeyboardMovement(float delta)
        {
            var crntAcceleration = new Vector3(SimInput.Manager.GetActionValue("camera/move_left_right"),
                SimInput.Manager.GetActionValue("camera/move_down_up"),
                -SimInput.Manager.GetActionValue("camera/move_backward_forward")) * Acceleration;

            velocity += crntAcceleration * delta;
            velocity = velocity.LimitLength(MaxSpeed);

            if (crntAcceleration.x == 0) velocity.x = Utils.ConvergeValue(velocity.x, 0, Acceleration * delta);
            if (crntAcceleration.y == 0) velocity.y = Utils.ConvergeValue(velocity.y, 0, Acceleration * delta);
            if (crntAcceleration.z == 0) velocity.z = Utils.ConvergeValue(velocity.z, 0, Acceleration * delta);

            Translation += Transform.basis.Xform(velocity * delta);
        }

        public override void _UnhandledInput(InputEvent _event)
        {
            if (!Current) return;

            rotationManager.UnhandledInput(_event);
        }

        public override void BeforeActivated()
        {
            var camera = GetViewport().GetCamera();
            GlobalTranslation = camera.GlobalTranslation;

            rotationManager.SetPanAndTilt(camera.GlobalRotation.x, camera.GlobalRotation.y);
        }
    }
}
