using Godot;
using System;

namespace Physics.Forcers
{
    public partial class WheelForcer : AbstractSpatialForcer
    {
        [Export] public NodePath DisplayObjectPath { get; set; }
        private Node3D displayObject;

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

        [Export] public PacejkaSettings LateralPacejka;
        [Export] public PacejkaSettings LongitudinalPacejka;

        // Nodes
        private SpringArm3D springArm;
        private RayCast3D rayCast;

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

            GetNode<Node3D>("SpringArm3D/CSGBox3D").QueueFree(); // delete visual widget

            displayObject = Utils.GetNodeWithWarnings<Node3D>(this, DisplayObjectPath, "display object");

            Utils.Assert(LongitudinalPacejka != null, "longitudinal pacejka is null", this);
            Utils.Assert(LateralPacejka != null, "lateral pacejka is null", this);

            springArm = GetNode<SpringArm3D>("SpringArm3D");
            rayCast = GetNode<RayCast3D>("SpringArm3D/RayCast3D");

            rayCast.TargetPosition = Vector3.Down * WheelRadius * 5;

            var shape = new SphereShape3D();
            shape.Radius = WheelRadius;
            springArm.Shape = shape;
            springArm.SpringLength = SuspensionTravel;
        }

        public override void _Process(double delta)
        {
            if (displayObject != null)
            {
                displayObject.RotateX(-angularVelocity * (float)delta);
                displayObject.GlobalPosition = rayCast.GlobalPosition;
            }
        }

        public override void Apply(PhysicsDirectBodyState3D state)
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

                var contactPoint = rayCast.GetCollisionPoint() - state.Transform.Origin;

                // tire
                var localVelocity = GlobalTransform.Basis.Inverse() * ((GlobalPosition - prevPosition) / state.Step);
                var zVel = -localVelocity.Y; // because raycast needs to be rotated
                var directionVector = new Vector2(localVelocity.X, localVelocity.Y).Normalized();
                prevPosition = GlobalPosition;

                var xSlip = Mathf.Asin(Mathf.Clamp(-directionVector.X, -1, 1));
                var xForce = Pacejka(springForce, xSlip, LateralPacejka);

                float zSlip = 0;
                if (zVel != 0) zSlip = (WheelRadius * angularVelocity - zVel) / Mathf.Abs(zVel);

                var zForce = Pacejka(springForce, zSlip, LongitudinalPacejka);

                // Add all the forces
                state.ApplyForce(GlobalTransform.Basis.X * xForce, contactPoint);
                state.ApplyForce(springForce * rayCast.GetCollisionNormal(), contactPoint);
                // state.ApplyForce(-GlobalTransform.Basis.Y * zForce, contactPoint);

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