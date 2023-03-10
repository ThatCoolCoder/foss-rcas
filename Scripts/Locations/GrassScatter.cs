using Godot;
using System;

namespace Locations
{
    public class GrassScatter : MultiMeshInstance
    {
        // Thing that scatters grass within a rectangular region while also respecting a mask.
        // Configure the rectangular region by scaling this.
        // Probably is a dumb way to do things but I can't think of anything better
        // Potential improvement: make it a tool script and make the mask editable in the editor.

        [Export] public int InstanceCount { get; set; } = 100;
        [Export] public Texture Mask { get; set; } // Only on white regions of this texture is grass spawned. If you leave it out then it's just everywhere
        [Export] public Texture Texture { get; set; }
        [Export] public Vector2 GrassSize { get; set; } = new Vector2(0.07f, 0.5f);
        [Export] public Vector2 GrassSizeVariation { get; set; } = Vector2.One * 0.3f; // varies by +- this amount

        private const int maxMaskTries = 100; // If we fail to find a position that satisfies the mask in this many tries, give up

        public override void _Ready()
        {
            GetNode<Spatial>("CSGBox").Visible = false;

            var size = Scale * 2;
            GetNode<Spatial>("CSGBox").Scale = Scale;
            Scale = Vector3.One;

            Multimesh = new();
            Multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
            var mesh = new QuadMesh();
            mesh.Size = GrassSize;
            Multimesh.Mesh = mesh;

            var material = new SpatialMaterial();
            material.AlbedoTexture = Texture;
            material.FlagsTransparent = true;
            mesh.Material = material;
            material.ParamsCullMode = SpatialMaterial.CullMode.Disabled;

            int trueInstanceCount = (int)(InstanceCount * SimSettings.Settings.Current.Graphics.VegetationMultiplier);

            Multimesh.InstanceCount = trueInstanceCount;
            var minPos = new Vector3(-size.x / 2, 0, -size.z / 2);
            var maxPos = new Vector3(size.x / 2, 0, size.z / 2);

            var maskImage = Mask == null ? null : Mask.GetData();
            if (maskImage != null) maskImage.Lock();
            for (int i = 0; i < trueInstanceCount; i++)
            {
                var transform = Transform.Identity;
                transform = transform.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau);
                transform.basis.Scale = (new Vector3(
                    ((float)GD.RandRange(-GrassSizeVariation.x, GrassSizeVariation.x) + 1),
                    (float)GD.RandRange(-GrassSizeVariation.y, GrassSizeVariation.y) + 1,
                    (float)GD.RandRange(-GrassSizeVariation.x, GrassSizeVariation.x) + 1
                ));
                for (int attemptNum = 0; attemptNum < maxMaskTries; attemptNum++)
                {
                    var pos = VectorExtensions.Random(minPos, maxPos);
                    var isOk = true;
                    if (Mask != null)
                    {
                        var x = (pos.x / size.x + 0.5f) * Mask.GetWidth();
                        var y = (pos.z / size.z + 0.5f) * Mask.GetHeight();
                        isOk = maskImage.GetPixel((int)x, (int)y).r > 0.5f;
                    }
                    if (isOk)
                    {
                        transform.origin = pos.WithY(transform.basis.Scale.y * GrassSize.y / 2);
                        Multimesh.SetInstanceTransform(i, transform);
                        break;
                    }
                }
            };
        }
    }
}