using Godot;
using System;

namespace Locations
{
    public class ImpostorTreeField : MultiMeshInstance
    {
        // Thing that scatters a TON of impostor trees within a rectangular region.
        // Each tree is a quad + a texture
        // No config is needed beyond setting these export parameters.
        // Configure the size by scaling it

        [Export] public int Count { get; set; } = 100;
        [Export] public bool UseNearScaling { get; set; } = true; // whether to use the near or far scaling for vegetation amount
        [Export] public Texture Texture { get; set; }
        [Export] public float AlphaScissorThreshold { get; set; } = 0.2f;
        [Export] public Vector2 TreeSize { get; set; } = Vector2.One * 5;
        [Export] public float HeightVariation { get; set; } = 0.3f; // varies by +- this amount
        [Export] public float WidthRatioVariation { get; set; } = 0.25f;

        public override void _Ready()
        {
            GetNode<Spatial>("CSGBox").Visible = false;

            var size = Scale * 2;
            GetNode<Spatial>("CSGBox").Scale = Scale;
            Scale = Vector3.One;

            Multimesh = new();
            Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
            var mesh = new QuadMesh();
            mesh.Size = TreeSize;
            Multimesh.Mesh = mesh;

            var material = new SpatialMaterial();
            material.AlbedoTexture = Texture;
            material.FlagsTransparent = true;
            mesh.Material = material;
            material.ParamsCullMode = SpatialMaterial.CullMode.Disabled;
            material.ParamsUseAlphaScissor = true;
            material.ParamsAlphaScissorThreshold = AlphaScissorThreshold;

            var g = SimSettings.Settings.Current.Graphics;
            var trueCount = (int)(Count * (UseNearScaling ? g.NearVegetationMultiplier : g.FarVegetationMultiplier));

            Multimesh.InstanceCount = trueCount;
            var minPos = new Vector3(-size.x / 2, 0, -size.z / 2);
            var maxPos = new Vector3(size.x / 2, 0, size.z / 2);
            for (int i = 0; i < trueCount; i++)
            {
                var transform = Transform.Identity;
                transform = transform.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau);

                var heightScale = (float)GD.RandRange(-HeightVariation, HeightVariation) + 1;
                var widthScale = heightScale * (float)GD.RandRange(1 - WidthRatioVariation, 1 + WidthRatioVariation);

                transform.basis.Scale = new Vector3(widthScale, heightScale, widthScale);
                transform.origin = VectorExtensions.Random(minPos, maxPos).WithY(transform.basis.Scale.y * TreeSize.y / 2);
                Multimesh.SetInstanceTransform(i, transform);
            };
        }
    }
}