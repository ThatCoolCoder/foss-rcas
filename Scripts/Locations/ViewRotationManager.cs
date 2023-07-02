using Godot;
using System;

namespace Locations
{
    public class ViewRotationManager
    {
        // Thing that keeps track of a pan/tilt rotation and handles rotating a spatial to there.
        // If you want to do things manually then just use GetPanAndTilt
        // Has rotation from mouse and also siminput actions.
        // Call Update every frame and also call Input to make the mouse stuff work.
        // Update and Input return bools stating whether view was changed, which allows using this to snap out of a default view

        public bool MouseRotationEnabled { get; set; } = true;
        public bool KeyboardRotationEnabled { get; set; } = true;
        public Spatial Target { get; set; }

        public float MaxAngularSpeedDegrees { get; set; } = 120;
        public float AngularAccelerationDegrees { get; set; } = 480;
        public float MouseSensitivity { get; set; } = 1;

        private Vector2 crntRotation;
        private Vector2 angularVelocityDegrees;
        private Vector2 crntDragRotation;
        private Vector2? clickStartPos;

        public void SetPanAndTilt(float pan, float tilt)
        {
            crntRotation = new Vector2(pan, tilt);
            crntDragRotation = Vector2.Zero;
        }

        public (float, float) GetPanAndTilt()
        {
            return (crntRotation.x + crntDragRotation.x, crntRotation.y + crntDragRotation.y);
        }

        public bool Update(float delta)
        {
            var changed = false;

            if (KeyboardRotationEnabled)
            {
                changed = KeyboardRotation(delta);
            }

            if (Target != null)
            {
                Target.Rotation = new Vector3(crntRotation.x + crntDragRotation.x, crntRotation.y + crntDragRotation.y, 0);
            }

            return changed;
        }

        private bool KeyboardRotation(float delta)
        {
            // Returns whether input was done

            var crntAcceleration = new Vector2(-SimInput.Manager.GetActionValue("camera/tilt_combined"),
                -SimInput.Manager.GetActionValue("camera/pan_combined")) * AngularAccelerationDegrees;

            angularVelocityDegrees += crntAcceleration * delta;
            angularVelocityDegrees.x = Mathf.Clamp(angularVelocityDegrees.x, -MaxAngularSpeedDegrees, MaxAngularSpeedDegrees);
            angularVelocityDegrees.y = Mathf.Clamp(angularVelocityDegrees.y, -MaxAngularSpeedDegrees, MaxAngularSpeedDegrees);

            if (crntAcceleration.x == 0) angularVelocityDegrees.x = Utils.ConvergeValue(angularVelocityDegrees.x, 0, AngularAccelerationDegrees * delta);
            if (crntAcceleration.y == 0) angularVelocityDegrees.y = Utils.ConvergeValue(angularVelocityDegrees.y, 0, AngularAccelerationDegrees * delta);

            crntRotation.x = Mathf.Clamp(crntRotation.x + Mathf.Deg2Rad(angularVelocityDegrees.x * delta), -90, 90);
            crntRotation.y = crntRotation.y + Mathf.Deg2Rad(angularVelocityDegrees.y * delta);

            return crntAcceleration.LengthSquared() > 0;
        }

        public bool Input(InputEvent _event)
        {
            if (!MouseRotationEnabled) return false;

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
                return true;
            }
            else if (_event is InputEventMouseMotion motionEvent)
            {
                if (clickStartPos != null)
                {
                    var movement = motionEvent.Position - (Vector2)clickStartPos;
                    crntDragRotation.x = -movement.y * MouseSensitivity * 0.001f;
                    crntDragRotation.y = -movement.x * MouseSensitivity * 0.001f;
                    return true;
                }
                else return false;
            }
            return false;
        }
    }
}