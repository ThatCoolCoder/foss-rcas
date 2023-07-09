using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public partial class Propeller : AbstractSpatialFluidForcer
    {
        [Export] public float DiameterInches { get; set; } = 10; // Inches were used for diameter and pitch because that's what all the propeller manufacturers use.
        public float RadiusMetres
        {
            get
            {
                return DiameterInches / 2 * 2.54f / 100;
            }
        }
        [Export] public float PitchInches { get; set; } = 6;
        public float PitchMetres
        {
            get
            {
                return PitchInches * 2.54f / 100;
            }
        }
        [Export] public bool Clockwise { get; set; } = true;
        [Export] public float EfficiencyFactor { get; set; } = 0.6f; // Set this such that the calculated static thrust matches what is measured in real life
        [Export] public float LiftToDrag { get; set; } = 5; // Can tweak this to adjust current, torque and speed in various ways
        [Export] public float Mass { get; set; } = 0.028f; // Used for moment of inertia calculations
        public float AngularVelocity { get; set; } = 0; // (radians per second)
        public float Rpm
        {
            get
            {
                return AngularVelocity / Mathf.Tau * 60;
            }
        }
        public float LastExitSpeed { get; private set; } // last exit speed, relative to world
        public Vector3 LastEntryVelocity { get; private set; } // hacky thing we need to make propwash work with wind
        public float LastThrustMagnitude { get; private set; }
        private float currentTorques;
        private float currentBrakingTorques;

        public override void _Ready()
        {
            base._Ready();
            AddToGroup("Propeller");
        }

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState3D state)
        {
            return Vector3.Zero; // convtodo: enable this forcer

            var rps = AngularVelocity / Mathf.Tau;
            var exitSpeed = PitchMetres * rps;

            var density = fluid.DensityAtPoint(GlobalPosition);
            LastEntryVelocity = fluid.VelocityAtPoint(GlobalPosition);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - LastEntryVelocity;
            var localVelocity = GlobalTransform.Basis.Inverse() * relativeVelocity;
            var entrySpeed = -localVelocity.Z;
            var deltaSpeed = exitSpeed - entrySpeed;

            LastExitSpeed = deltaSpeed;

            var area = Mathf.Pi * RadiusMetres * RadiusMetres;

            var force = .5f * density * area * (exitSpeed * exitSpeed - entrySpeed * entrySpeed);

            if (!Clockwise) force = -force;
            if (Rpm < 0) force = -force;

            // Propellers are roughly half as efficient when being used backwards.
            if (force < 0) force *= 0.5f;


            force *= EfficiencyFactor;

            LastThrustMagnitude = force;

            return -GlobalTransform.Basis.Z * force;
        }

        private float CalculateAirResistance()
        {
            // A rough approximation, todo: do this better later

            var dragForce = -1 * LastThrustMagnitude / LiftToDrag;
            if (!Clockwise) dragForce = -dragForce;
            var torqueDistance = RadiusMetres * .5f;
            return torqueDistance * dragForce;
        }

        public override void _PhysicsProcess(double delta)
        {
            AddConstantTorque(CalculateAirResistance());
            var momentOfInertia = Mass * RadiusMetres * RadiusMetres / 3;
            AngularVelocity += currentTorques / momentOfInertia * (float)delta;
            var brakingAcceleration = currentBrakingTorques / momentOfInertia * (float)delta;
            if (Mathf.Abs(brakingAcceleration) > Mathf.Abs(AngularVelocity))
            {
                AngularVelocity = 0;
            }
            else
            {
                AngularVelocity -= brakingAcceleration * Mathf.Sign(AngularVelocity);
            }

            currentTorques = 0;
            currentBrakingTorques = 0;

            RotateZ(-AngularVelocity * (float)delta); // (In godot a negative rotation along -z is clockwise)
        }

        public void AddConstantTorque(float torque)
        {
            currentTorques += torque;
        }

        public void AddBrakingTorque(float torque)
        {
            currentBrakingTorques += torque;
        }

        public void HitObject()
        {
            // Scenes with a propeller should have an area3d that triggers this when the prop hits stuff.
            AngularVelocity = 0;
        }
    }
}