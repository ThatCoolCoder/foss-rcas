using Godot;
using System;

namespace Locations
{
    public class FreeCamera : BasicFlightCamera
    {
        // Keyboard movement stuff
        private Vector3 velocity; // (in local space)
        [Export] public float MaxSpeed { get; set; } = 10;
        [Export] public float Acceleration { get; set; } = 40;

        // Keyboard rotation stuff
        private Vector2 crntRotation;
        private Vector2 angularVelocityDegrees;
        [Export] public float MaxAngularSpeedDegrees { get; set; } = 120;
        [Export] public float AngularAccelerationDegrees { get; set; } = 480;

        // Mouse rotation stuff
        [Export] public float MouseSensitivity { get; set; } = 1;
        private Vector2 crntDragRotation;
        private Vector2? clickStartPos;

        public override void _Process(float delta)
        {
            if (Current)
            {
                KeyboardMovement(delta);
                KeyboardRotation(delta);

                Rotation = new Vector3(crntRotation.x + crntDragRotation.x, crntRotation.y + crntDragRotation.y, 0);
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

        private void KeyboardRotation(float delta)
        {
            var crntAcceleration = new Vector2(-SimInput.Manager.GetActionValue("camera/tilt_combined"),
                -SimInput.Manager.GetActionValue("camera/pan_combined")) * AngularAccelerationDegrees;

            angularVelocityDegrees += crntAcceleration * delta;
            angularVelocityDegrees.x = Mathf.Clamp(angularVelocityDegrees.x, -MaxAngularSpeedDegrees, MaxAngularSpeedDegrees);
            angularVelocityDegrees.y = Mathf.Clamp(angularVelocityDegrees.y, -MaxAngularSpeedDegrees, MaxAngularSpeedDegrees);

            if (crntAcceleration.x == 0) angularVelocityDegrees.x = Utils.ConvergeValue(angularVelocityDegrees.x, 0, AngularAccelerationDegrees * delta);
            if (crntAcceleration.y == 0) angularVelocityDegrees.y = Utils.ConvergeValue(angularVelocityDegrees.y, 0, AngularAccelerationDegrees * delta);

            crntRotation.x = Mathf.Clamp(crntRotation.x + Mathf.Deg2Rad(angularVelocityDegrees.x * delta), -90, 90);
            crntRotation.y = crntRotation.y + Mathf.Deg2Rad(angularVelocityDegrees.y * delta);
        }

        public override void _Input(InputEvent _event)
        {
            if (!Current) return;

            if (_event is InputEventMouseButton clickEvent)
            {
                if (clickEvent.Pressed)
                {
                    clickStartPos = clickEvent.Position;
                }
                else
                {
                    crntRotation.x += crntDragRotation.x;
                    crntRotation.y += crntDragRotation.y;
                    crntDragRotation = Vector2.Zero;
                    clickStartPos = null;
                }
            }
            else if (_event is InputEventMouseMotion motionEvent)
            {
                if (clickStartPos != null)
                {
                    var movement = motionEvent.Position - (Vector2)clickStartPos;
                    crntDragRotation.x = -movement.y * MouseSensitivity * 0.001f;
                    crntDragRotation.y = -movement.x * MouseSensitivity * 0.001f;
                }
            }
        }

        public override void BeforeActivated()
        {
            var camera = GetViewport().GetCamera();
            GlobalTranslation = camera.GlobalTranslation;

            crntRotation.x = camera.GlobalRotation.x;
            crntRotation.y = camera.GlobalRotation.y;
        }
    }
}
