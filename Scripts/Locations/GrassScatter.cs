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
        // todo: Is a bit hacky, could do with some refactoring

        [Export] public bool FalloffAroundCamera { get; set; } = true; // Use the camera as a central point for falloff?
        [Export] public float FalloffMaxDistance { get; set; } = 100;
        [Export] public float CameraMoveDistBeforeUpdate { get; set; } = 20; // Update grass falloff when camera moves this far
        [Export] public int InstanceCount { get; set; } = 100;
        [Export] public Texture Mask { get; set; } // Only on white regions of this texture is grass spawned. If you leave it out then it's just everywhere
        [Export] public Texture Texture { get; set; }
        [Export] public Texture NormalMap { get; set; }
        [Export] public Vector2 GrassSize { get; set; } = new Vector2(0.07f, 0.5f);
        [Export] public Vector2 GrassSizeVariation { get; set; } = Vector2.One * 0.3f; // varies by +- this amount

        // If we fail to find a position that satisfies the mask in this many tries, give up.
        // If grass takes too long to generate try reducing this
        [Export] public int MaxMaskTries { get; set; } = 100;

        private Vector3 size;
        private Thread generateGrassThread;
        private Vector3 lastUpdatePos; // position of camera upon last update if using 

        public override void _Ready()
        {
            // Clean up the editor visualisations
            GetNode<Spatial>("CSGBox").Visible = false;

            size = Scale * 2;
            GetNode<Spatial>("CSGBox").Scale = Scale;
            Scale = Vector3.One;

            // GenerateGrass();
        }

        private void GenerateGrass()
        {
            // todo: probably we could move some of this code to ready()
            MultiMesh multimesh = new();
            multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;
            var mesh = new QuadMesh();
            mesh.Size = GrassSize;
            multimesh.Mesh = mesh;

            var material = new SpatialMaterial();
            material.AlbedoTexture = Texture;
            material.FlagsTransparent = true;
            mesh.Material = material;
            material.ParamsCullMode = SpatialMaterial.CullMode.Disabled;
            material.FlagsUnshaded = true;
            if (NormalMap == null) material.NormalEnabled = false;
            else
            {
                material.NormalEnabled = true;
                material.NormalTexture = NormalMap;
            }

            int trueInstanceCount = (int)(InstanceCount * SimSettings.Settings.Current.Graphics.VegetationMultiplier);

            multimesh.InstanceCount = trueInstanceCount;
            var minPos = new Vector3(-size.x / 2, 0, -size.z / 2);
            var maxPos = new Vector3(size.x / 2, 0, size.z / 2);

            var cameraPos = GetCameraPos();
            lastUpdatePos = cameraPos;

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
                for (int attemptNum = 0; attemptNum < MaxMaskTries; attemptNum++)
                {
                    Vector3 pos;
                    if (FalloffAroundCamera)
                    {
                        var distMultiplier = GD.Randf();
                        var relativeToCamera = Vector3.Forward.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau) * distMultiplier * FalloffMaxDistance;
                        pos = ToLocal(relativeToCamera + cameraPos);

                        if (Mathf.Abs(pos.x) > (size.x / 2) || Mathf.Abs(pos.z) > (size.z / 2)) continue;
                    }
                    else
                    {
                        pos = VectorExtensions.Random(minPos, maxPos);
                    }

                    // check if within mask
                    if (Mask != null)
                    {
                        var x = (pos.x / size.x + 0.5f) * Mask.GetWidth();
                        var y = (pos.z / size.z + 0.5f) * Mask.GetHeight();
                        if (maskImage.GetPixel((int)x, (int)y).r < 0.5f) continue;
                    }
                    transform.origin = pos.WithY(transform.basis.Scale.y * GrassSize.y / 2);
                    multimesh.SetInstanceTransform(i, transform);
                    break;
                }
            };

            Multimesh = multimesh;
        }

        private void GenerateGrassOnThread()
        {
            if (generateGrassThread == null || !generateGrassThread.IsAlive())
            {
                generateGrassThread = new Thread();
                generateGrassThread.Start(this, "GenerateGrass");
            }
        }

        private Vector3 GetCameraPos()
        {
            return GetViewport().GetCamera().GlobalTranslation;
        }

        public override void _Process(float delta)
        {
            if (Engine.GetFramesDrawn() % 30 == 0 &&
                FalloffAroundCamera &&
                GetCameraPos().DistanceSquaredTo(lastUpdatePos) > CameraMoveDistBeforeUpdate * CameraMoveDistBeforeUpdate)
            {
                GenerateGrassOnThread();
            }
        }

        public override void _ExitTree()
        {
            if (generateGrassThread != null) generateGrassThread.WaitToFinish();
        }
    }
}