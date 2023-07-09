using Godot;
using System;

namespace Locations
{
    public partial class ImpostorTreeField : MultiMeshInstance3D
    {
        // Thing that scatters a TON of impostor trees within a rectangular region.
        // Each tree is a quad + a texture
        // No config is needed beyond setting these export parameters.
        // Configure the size by scaling it

        [Export] public int Count { get; set; } = 100;
        [Export] public bool UseNearScaling { get; set; } = true; // whether to use the near or far scaling for vegetation amount
        [Export] public Texture2D Texture2D { get; set; }
        [Export] public float AlphaScissorThreshold { get; set; } = 0.2f;
        [Export] public Vector2 TreeSize { get; set; } = Vector2.One * 5;
        [Export] public float HeightVariation { get; set; } = 0.3f; // varies by +- this amount
        [Export] public float WidthRatioVariation { get; set; } = 0.25f;

        public override void _Ready()
        {
            GetNode<Node3D>("CSGBox3D").Visible = false;

            var size = Scale * 2;
            GetNode<Node3D>("CSGBox3D").Scale = Scale;
            Scale = Vector3.One;

            Multimesh = new();
            Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
            var mesh = new QuadMesh();
            mesh.Size = TreeSize;
            Multimesh.Mesh = mesh;

            var material = new StandardMaterial3D();
            material.AlbedoTexture = Texture2D;
            material.Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor;
            material.AlphaScissorThreshold = AlphaScissorThreshold;
            mesh.Material = material;
            material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;

            var g = SimSettings.Settings.Current.Graphics;
            var trueCount = (int)(Count * (UseNearScaling ? g.NearVegetationMultiplier : g.FarVegetationMultiplier));

            Multimesh.InstanceCount = trueCount;
            var minPos = new Vector3(-size.X / 2, 0, -size.Z / 2);
            var maxPos = new Vector3(size.X / 2, 0, size.Z / 2);
            for (int i = 0; i < trueCount; i++)
            {
                var transform = Transform3D.Identity;
                transform = transform.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau);

                var heightScale = (float)GD.RandRange(-HeightVariation, HeightVariation) + 1;
                var widthScale = heightScale * (float)GD.RandRange(1 - WidthRatioVariation, 1 + WidthRatioVariation);

                // transform.Basis.Scale = new Vector3(widthScale, heightScale, widthScale); // convtodo: need to figure out how to set a scale
                transform.Origin = VectorExtensions.Random(minPos, maxPos).WithY(transform.Basis.Scale.Y * TreeSize.Y / 2);
                Multimesh.SetInstanceTransform(i, transform);
            };
        }
    }
}