using Godot;
using System;

namespace Locations
{
    public partial class ViewRotationManager
    {
        // Thing that keeps track of a pan/tilt rotation and handles rotating a spatial to there.
        // If you want to do things manually then just use GetPanAndTilt
        // Has rotation from mouse and also siminput actions.
        // Call Update every frame and also call Input to make the mouse stuff work.
        // Update and Input return bools stating whether view was changed, which allows using this to snap out of a default view

        public bool MouseRotationEnabled { get; set; } = true;
        public bool KeyboardRotationEnabled { get; set; } = true;
        public Node3D Target { get; set; }

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
            return (crntRotation.X + crntDragRotation.X, crntRotation.Y + crntDragRotation.Y);
        }

        public bool Update(double delta)
        {
            var changed = false;

            if (KeyboardRotationEnabled)
            {
                changed = KeyboardRotation((float)delta);
            }

            if (Target != null)
            {
                Target.Rotation = new Vector3(crntRotation.X + crntDragRotation.X, crntRotation.Y + crntDragRotation.Y, 0);
            }

            return changed;
        }

        private bool KeyboardRotation(float delta)
        {
            // Returns whether input was done

            var crntAcceleration = new Vector2(-SimInput.Manager.GetActionValue("camera/tilt_combined"),
                -SimInput.Manager.GetActionValue("camera/pan_combined")) * AngularAccelerationDegrees;

            angularVelocityDegrees += crntAcceleration * delta;
            angularVelocityDegrees.X = Mathf.Clamp(angularVelocityDegrees.X, -MaxAngularSpeedDegrees, MaxAngularSpeedDegrees);
            angularVelocityDegrees.Y = Mathf.Clamp(angularVelocityDegrees.Y, -MaxAngularSpeedDegrees, MaxAngularSpeedDegrees);

            if (crntAcceleration.X == 0) angularVelocityDegrees.X = Utils.ConvergeValue(angularVelocityDegrees.X, 0, AngularAccelerationDegrees * delta);
            if (crntAcceleration.Y == 0) angularVelocityDegrees.Y = Utils.ConvergeValue(angularVelocityDegrees.Y, 0, AngularAccelerationDegrees * delta);

            crntRotation.X = Mathf.Clamp(crntRotation.X + Mathf.DegToRad(angularVelocityDegrees.X * delta), -90, 90);
            crntRotation.Y = crntRotation.Y + Mathf.DegToRad(angularVelocityDegrees.Y * delta);

            return crntAcceleration.LengthSquared() > 0;
        }

        public bool UnhandledInput(InputEvent _event)
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
                    crntRotation.X += crntDragRotation.X;
                    crntRotation.Y += crntDragRotation.Y;
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
                    crntDragRotation.X = -movement.Y * MouseSensitivity * 0.001f;
                    crntDragRotation.Y = -movement.X * MouseSensitivity * 0.001f;
                    return true;
                }
                else return false;
            }
            return false;
        }
    }
}