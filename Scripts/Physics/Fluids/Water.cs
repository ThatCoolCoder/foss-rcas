using System;
using Godot;

namespace Physics.Fluids
{
    public partial class Water : MeshInstance3D, ISpatialFluid
    {
        // todo: implement water "physics" in shader and in c# 
        // todo: make this a tool and configurable in editor without messing with the mesh's inner properties

        [Export] public float BaseDensity { get; set; } = 1000.0f;
        [Export] public Vector3 Flow { get; set; } = Vector3.Zero;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(double delta)
        {

            base._Process(delta);
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.Y <= BoundaryAtPoint(point).Y;
        }

        public Vector3 BoundaryAtPoint(Vector3 point)
        {
            return point.WithY(GlobalPosition.Y);
        }


        public float DensityAtPoint(Vector3 point)
        {
            return BaseDensity;
        }

        public Vector3 VelocityAtPoint(Vector3 point)
        {
            return Flow;
        }

        public Vector3 NormalAtPoint(Vector3 point)
        {
            return Vector3.Up;
        }

        public bool HasBoundaries { get; set; } = true;
    }
}
