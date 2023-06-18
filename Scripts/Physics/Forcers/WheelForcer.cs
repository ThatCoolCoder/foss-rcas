using Godot;
using System;

namespace Physics.Forcers
{
    public class WheelForcer : AbstractSpatialForcer
    {
        [Export] public NodePath DisplayObjectPath { get; set; }
        private Spatial displayObject;

        [Export] public float SuspensionTravel { get; set; } = .05f;
        [Export] public float SuspensionStrength { get; set; } = 1;
        [Export] public float SuspensionDamping { get; set; } = .5f;

        [Export] public float WheelRadius { get; set; } = 0.05f;
        [Export] public float WheelMass { get; set; } = 0.02f;
        [Export] public float WheelFrictionTorque { get; set; } = .1f; // use this to add a bit of friction to wheels so they don't spin forever
        [Export] public float WheelDriveTorque { get; set; } = 1;
        public float WheelDriveFactor { get; set; } = 0; // +ve = forwards, -ve = reverse
        [Export] public float WheelBrakeTorque { get; set; } = 1;
        public float WheelBrakeFactor { get; set; } = 0;

        [Export(PropertyHint.File, "*.tres")] public string LateralPacejkaPath { get; set; } = null;
        private PacejkaSettings lateralPacejka;
        [Export(PropertyHint.File, "*.tres")] public string LongitudinalPacejkaPath { get; set; } = null;
        private PacejkaSettings longitudinalPacejka;

        // Nodes
        private SpringArm springArm;
        private RayCast rayCast;

        // State
        private float prevCompression;
        private Vector3 prevPosition;
        private float prevZForce;
        private float angularVelocity;
        private float momentOfInertia
        {
            get
            {
                return .5f * WheelMass * WheelRadius * WheelRadius;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            GetNode<Spatial>("SpringArm/CSGBox").QueueFree(); // delete visual widget

            if (LateralPacejkaPath != null) lateralPacejka = ResourceLoader.Load<PacejkaSettings>(LateralPacejkaPath);
            if (LongitudinalPacejkaPath != null) longitudinalPacejka = ResourceLoader.Load<PacejkaSettings>(LongitudinalPacejkaPath);

            displayObject = Utils.GetNodeWithWarnings<Spatial>(this, DisplayObjectPath, "display object");

            springArm = GetNode<SpringArm>("SpringArm");
            rayCast = GetNode<RayCast>("SpringArm/RayCast");

            rayCast.CastTo = Vector3.Down * WheelRadius * 5;

            var shape = new SphereShape();
            shape.Radius = WheelRadius;
            springArm.Shape = shape;
            springArm.SpringLength = SuspensionTravel;
        }

        public override void _Process(float delta)
        {
            if (displayObject != null)
            {
                displayObject.RotateX(-angularVelocity * delta);
                displayObject.GlobalTranslation = rayCast.GlobalTranslation;
            }
        }

        public override void Apply(PhysicsDirectBodyState state)
        {
            // todo: this is somewhat messy, perhaps this should be split up or part of it moved out of apply into process

            // Torques, inertia

            var finalBrakeTorque = WheelBrakeFactor * WheelBrakeTorque + WheelFrictionTorque;

            var netTorque = WheelDriveFactor * WheelDriveTorque + (WheelRadius * prevZForce);
            var pureBraking = finalBrakeTorque > Mathf.Abs(netTorque);
            netTorque -= finalBrakeTorque * Mathf.Sign(angularVelocity);

            if (pureBraking)
            {
                // Apply pure braking, converge to 0
                angularVelocity = Utils.ConvergeValue(angularVelocity, 0, Mathf.Abs(netTorque) / momentOfInertia * state.Step);
            }
            else
            {
                // Don't converge to 0
                angularVelocity += netTorque / momentOfInertia * state.Step;
            }

            if (rayCast.IsColliding())
            {
                // Suspension
                var compression = 1 - (springArm.GetHitLength() / springArm.SpringLength);
                var springForce = SuspensionStrength * compression;
                springForce += SuspensionDamping * (compression - prevCompression) / state.Step;

                var contactPoint = rayCast.GetCollisionPoint() - state.Transform.origin;

                // tire
                var localVelocity = GlobalTransform.basis.XformInv((GlobalTranslation - prevPosition) / state.Step);
                var zVel = -localVelocity.y; // because raycast needs to be rotated
                var directionVector = new Vector2(localVelocity.x, localVelocity.y).Normalized();
                prevPosition = GlobalTranslation;

                var xSlip = Mathf.Asin(Mathf.Clamp(-directionVector.x, -1, 1));
                var xForce = Pacejka(springForce, xSlip, lateralPacejka);

                float zSlip = 0;
                if (zVel != 0) zSlip = (WheelRadius * angularVelocity - zVel) / Mathf.Abs(zVel);

                var zForce = Pacejka(springForce, zSlip, longitudinalPacejka);

                // Add all the forces
                state.AddForce(GlobalTransform.basis.x * xForce, contactPoint);
                state.AddForce(springForce * rayCast.GetCollisionNormal(), contactPoint);
                // state.AddForce(-GlobalTransform.basis.y * zForce, contactPoint);

                prevZForce = -zForce;
                prevCompression = compression;
            }
            else
            {
                prevZForce = 0;
                prevCompression = 0;
            }
        }

        private float Pacejka(float normalForce, float slip, PacejkaSettings s)
        {
            // Pacejka tire formula
            return normalForce * s.Peak * Mathf.Sin(s.Shape * Mathf.Atan(s.Stiff * slip - s.Curve * s.Stiff * slip - Mathf.Atan(s.Stiff * slip)));
        }
    }
}