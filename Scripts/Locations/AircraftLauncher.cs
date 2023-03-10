using Godot;
using System;

namespace Locations
{
    public class AircraftLauncher : Spatial
    {
        public class LauncherSettings
        {
            public float Height { get; set; } = 1.5f;
            public float Speed { get; set; } = 10;
            public float Angle { get; set; } = 0;
        }

        public LauncherSettings Settings { get; set; }
        private RigidBody target;
        private bool used = false;

        public void SetTarget(RigidBody newTarget)
        {
            target = newTarget;
            target.Mode = RigidBody.ModeEnum.Static;
            used = false;

            target.LinearVelocity = Vector3.Zero;
            target.AngularVelocity = Vector3.Zero;
            target.GlobalTranslation = GlobalTranslation.WithY(GlobalTranslation.y + Settings.Height);
            target.Rotation = new Vector3(Settings.Angle, Rotation.y, 0);
        }

        public void Launch()
        {
            if (used || target == null) return;

            used = true;
            target.Mode = RigidBody.ModeEnum.Rigid;
            target.LinearVelocity = (Vector3.Forward * Settings.Speed).Rotated(Vector3.Right, Settings.Angle).Rotated(Vector3.Up, Rotation.y);
        }
    }
}