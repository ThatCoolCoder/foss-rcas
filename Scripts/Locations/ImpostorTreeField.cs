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
        [Export] public bool Near { get; set; } = true; // whether to use the near or far scaling for vegetation amount
        [Export] public Texture Texture { get; set; }
        [Export] public Vector2 TreeSize { get; set; } = Vector2.One * 5;
        [Export] public Vector2 TreeSizeVariation { get; set; } = Vector2.One * 0.3f; // varies by +- this amount

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
            material.ParamsDepthDrawMode = SpatialMaterial.DepthDrawMode.AlphaOpaquePrepass;

            var g = SimSettings.Settings.Current.Graphics;
            var trueCount = (int)(Count * (Near ? g.NearVegetationMultiplier : g.FarVegetationMultiplier));

            Multimesh.InstanceCount = trueCount;
            var minPos = new Vector3(-size.x / 2, 0, -size.z / 2);
            var maxPos = new Vector3(size.x / 2, 0, size.z / 2);
            for (int i = 0; i < trueCount; i++)
            {
                var transform = Transform.Identity;
                transform = transform.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau);
                transform.basis.Scale = (new Vector3(
                    ((float)GD.RandRange(-TreeSizeVariation.x, TreeSizeVariation.x) + 1),
                    (float)GD.RandRange(-TreeSizeVariation.y, TreeSizeVariation.y) + 1,
                    (float)GD.RandRange(-TreeSizeVariation.x, TreeSizeVariation.x) + 1
                ));
                transform.origin = VectorExtensions.Random(minPos, maxPos).WithY(transform.basis.Scale.y * TreeSize.y / 2);
                Multimesh.SetInstanceTransform(i, transform);
            };
        }
    }
}