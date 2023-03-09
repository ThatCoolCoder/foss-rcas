using Godot;
using System;

namespace Physics.Fluids
{
    public class Air : Spatial, ISpatialFluid
    {
        // Air, supports basic wind with gusting and direction changing

        [Export] public float DensityMultiplier { get; set; } = 1;

        public WindSettings WindSettings { get; set; } = new();

        private OpenSimplexNoise speedNoise = new();
        private OpenSimplexNoise directionNoise = new();
        private float time = 0;

        public override void _Ready()
        {
            speedNoise.Seed = 25;
            speedNoise.Persistence = 0.75f;
        }

        public override void _PhysicsProcess(float delta)
        {
            time += delta;
        }

        public float DensityAtPoint(Vector3 _point)
        {
            return 1.293f * DensityMultiplier;
        }

        public bool ContainsPoint(Vector3 _point)
        {
            return true; // air is everywhere, except underground or underwater, but air is so much less dense than those places that it doesn't matter
        }

        public Vector3 VelocityAtPoint(Vector3 _point)
        {
            var gustSpeed = (speedNoise.GetNoise1d(time * WindSettings.GustFrequency) / 2 + 0.5f) * WindSettings.GustSpeedDelta;
            var finalSpeed = WindSettings.Speed + gustSpeed;

            var directionDelta = (directionNoise.GetNoise1d(time * WindSettings.DirectionChangeFrequency)) * WindSettings.DirectionVariability;
            var finalDirection = WindSettings.Direction + directionDelta;

            var flow = new Vector3(finalSpeed, 0, 0).Rotated(Vector3.Up, finalDirection);

            return flow;
        }

        public Vector3 NormalAtPoint(Vector3 _point)
        {
            return Vector3.Up;
        }

        public FluidType Type { get; set; } = FluidType.Gas;


    }
}