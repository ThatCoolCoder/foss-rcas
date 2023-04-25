using Godot;
using System;
using System.Collections.Generic;


namespace Locations
{
    public class GrassScatter : MultiMeshInstance
    {
        // Thing that scatters grass within a rectangular region while also respecting a mask and only creating it within a certain distance of the camera.
        // Updates the grass when camera moves but tries not to jitter existing blades
        // Configure the rectangular region by scaling this.
        // Probably is a dumb way to do things but I can't think of anything better
        // Potential improvement: make it a tool script and make the mask editable in the editor.
        // todo: Is a bit hacky, could do with some refactoring

        [Export] public float FalloffMaxDistance { get; set; } = 100;
        [Export] public float CameraMoveDistBeforeUpdate { get; set; } = 20; // Update grass falloff when camera moves this far
        [Export] public int InstanceCount { get; set; } = 100;
        [Export] public Texture Mask { get; set; } // Only on white regions of this texture is grass spawned. If you leave it out then it's just everywhere
        [Export] public Texture Texture { get; set; }
        [Export] public Texture NormalMap { get; set; }
        [Export] public Vector2 GrassSize { get; set; } = new Vector2(0.07f, 0.5f);
        [Export] public Vector2 GrassSizeVariation { get; set; } = Vector2.One * 0.3f; // varies by +- this amount

        // If we fail to find a position that satisfies the mask in this many tries, give up.
        // Try reducing this if grass takes too long to generate
        [Export] public int MaxMaskTries { get; set; } = 100;

        public int trueInstanceCount
        {
            get
            {
                var g = SimSettings.Settings.Current.Graphics;
                return (int)(InstanceCount * g.GrassMultiplier * g.GrassDistanceMultiplier * g.GrassDistanceMultiplier);
            }
        }
        public float trueFalloffMaxDistance
        {
            get
            {
                return SimSettings.Settings.Current.Graphics.GrassDistanceMultiplier * FalloffMaxDistance;
            }
        }


        private Vector3 size;
        private Thread generateGrassThread;
        private Vector3 lastUpdatePos;
        private bool generatedInitialGrass = false;

        public override void _Ready()
        {
            // Clean up the editor visualisations
            GetNode<Spatial>("CSGBox").Visible = false;

            size = Scale * 2;
            GetNode<Spatial>("CSGBox").Scale = Scale;
            Scale = Vector3.One;
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

            material.DistanceFadeMaxDistance = 0;
            material.DistanceFadeMinDistance = trueFalloffMaxDistance;
            material.DistanceFadeMode = SpatialMaterial.DistanceFadeModeEnum.PixelAlpha;

            if (NormalMap == null) material.NormalEnabled = false;
            else
            {
                material.NormalEnabled = true;
                material.NormalTexture = NormalMap;
            }

            var cameraPos = GetCameraPos();
            lastUpdatePos = cameraPos;
            var positions = GenerateGrassPositionsV3(cameraPos);

            multimesh.InstanceCount = positions.Count;

            for (int i = 0; i < positions.Count; i++)
            {
                // Calculcate rotation + scale
                var transform = Transform.Identity;
                var pos = positions[i];
                transform = transform.Rotated(Vector3.Up, SemiRandomFloat(pos.x + pos.z) * Mathf.Tau);
                Func<float, float, float, float> getAxisScale = (float x, float z, float variation) => 1 + (SemiRandomFloat(x + z * 1.52f) - .5f) * 2 * variation;
                transform.basis.Scale = new Vector3(
                    getAxisScale(pos.x, pos.z, GrassSizeVariation.x),
                    getAxisScale(pos.x, pos.z, GrassSizeVariation.y),
                    getAxisScale(pos.x, pos.z, GrassSizeVariation.x)
                );

                // Adjust position then save
                transform.origin = pos.WithY(transform.basis.Scale.y * GrassSize.y / 2);
                multimesh.SetInstanceTransform(i, transform);
            }

            Multimesh = multimesh;
        }

        private List<Vector3> GenerateGrassPositionsV1(Vector3 falloffCenter)
        {
            // Create the positions for the grass
            // the blades may need to be moved up or down depending on their height

            // Most basic algorithm, just places them in a circle around the camera

            int targetInstanceCount = trueInstanceCount;

            var maskImage = Mask == null ? null : Mask.GetData();
            if (maskImage != null) maskImage.Lock();

            var positions = new List<Vector3>();

            for (int i = 0; i < targetInstanceCount; i++)
            {
                for (int attemptNum = 0; attemptNum < MaxMaskTries; attemptNum++)
                {
                    var distMultiplier = GD.Randf();
                    var relativeToCamera = Vector3.Forward.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau) * distMultiplier * trueFalloffMaxDistance;
                    var pos = ToLocal(relativeToCamera + falloffCenter);

                    // Check if within bounds
                    if (Mathf.Abs(pos.x) > (size.x / 2) || Mathf.Abs(pos.z) > (size.z / 2)) continue;

                    // Check if within mask
                    if (Mask != null)
                    {
                        var x = (pos.x / size.x + 0.5f) * Mask.GetWidth();
                        var y = (pos.z / size.z + 0.5f) * Mask.GetHeight();
                        if (maskImage.GetPixel((int)x, (int)y).r < 0.5f) continue;
                    }

                    positions.Add(pos);
                    break;
                }
            };

            // var culledPositions = 

            if (maskImage != null) maskImage.Unlock();

            return positions;
        }

        private List<Vector3> GenerateGrassPositionsV2(Vector3 falloffCenter)
        {
            // Create the positions for the grass
            // the blades may need to be moved up or down depending on their height

            // Overview of the algorithm used:
            // - Generate points in a grid pattern centered around the camera
            // - Deterministically jiggle those points within their "cells"
            // - Remove positions with a probability based on how far they are from the camera

            int targetInstanceCount = trueInstanceCount;
            int preCullInstanceCount = Mathf.FloorToInt(targetInstanceCount * 4 / Mathf.Pi); // todo: this only accounts for the hard cutoff at radius, not the fade
            int rowCount = Mathf.FloorToInt(Mathf.Sqrt(preCullInstanceCount));
            var gridSpacing = 2 * trueFalloffMaxDistance / Mathf.Sqrt(preCullInstanceCount);

            var maskImage = Mask == null ? null : Mask.GetData();
            if (maskImage != null) maskImage.Lock();

            var initialPositions = new List<Vector3>();

            // Generate initial positions
            for (int i = 0; i < preCullInstanceCount; i++)
            {
                for (int attemptNum = 0; attemptNum < MaxMaskTries; attemptNum++)
                {
                    int row = i / rowCount - rowCount / 2;
                    int column = i % rowCount - rowCount / 2;

                    var relativeToCamera = new Vector3(column, 0, row) * gridSpacing;
                    var pos = ToLocal(relativeToCamera + falloffCenter);

                    // Check if within bounds
                    if (Mathf.Abs(pos.x) > (size.x / 2) || Mathf.Abs(pos.z) > (size.z / 2)) continue;

                    // Check if within mask
                    if (Mask != null)
                    {
                        var x = (pos.x / size.x + 0.5f) * Mask.GetWidth();
                        var y = (pos.z / size.z + 0.5f) * Mask.GetHeight();
                        if (maskImage.GetPixel((int)x, (int)y).r < 0.5f) continue;
                    }

                    initialPositions.Add(pos);
                    break;
                }
            };

            if (maskImage != null) maskImage.Unlock();
            return initialPositions;
        }

        private List<Vector3> GenerateGrassPositionsV3(Vector3 falloffCenter)
        {
            // Create the positions for the grass
            // the blades may need to be moved up or down depending on their height

            // Overview of the algorithm used:
            // - Generate points in a circle around the camera, with exponential falloff
            // - Snap them to a grid
            // - Deterministically jiggle those points based on their grid position
            // - Remove points which do not satisfy the bounds and mask.

            var g = SimSettings.Settings.Current.Graphics;
            int targetInstanceCount = (int)(InstanceCount * g.GrassMultiplier * g.GrassDistanceMultiplier * g.GrassDistanceMultiplier);
            var gridSpacing = 2 * trueFalloffMaxDistance / Mathf.Sqrt(targetInstanceCount);

            var maskImage = Mask == null ? null : Mask.GetData();
            if (maskImage != null) maskImage.Lock();

            var positions = new List<Vector3>();

            for (int i = 0; i < targetInstanceCount; i++)
            {
                for (int attemptNum = 0; attemptNum < MaxMaskTries; attemptNum++)
                {
                    var distMultiplier = SemiRandomFloat(i * 2.235f + attemptNum);
                    var relativeToCamera = Vector3.Forward.Rotated(Vector3.Up, SemiRandomFloat(i * 4.554f + attemptNum) * Mathf.Tau) * distMultiplier * trueFalloffMaxDistance;
                    var pos = ToLocal(relativeToCamera + falloffCenter);

                    // Snap to grid
                    var snappedPos = new Vector3(Utils.RoundNumber(pos.x, gridSpacing), 0, Utils.RoundNumber(pos.z, gridSpacing));

                    // Jiggle
                    var jiggle = new Vector3(
                        SemiRandomFloat(pos.x + 3 * pos.z) * gridSpacing,
                        0,
                        SemiRandomFloat(3 * pos.x + pos.z) * gridSpacing);
                    var finalPos = snappedPos + jiggle;

                    // Check if within bounds
                    if (Mathf.Abs(finalPos.x) > (size.x / 2) || Mathf.Abs(finalPos.z) > (size.z / 2)) continue;

                    // Check if within mask
                    if (Mask != null)
                    {
                        var x = (finalPos.x / size.x + 0.5f) * Mask.GetWidth();
                        var y = (finalPos.z / size.z + 0.5f) * Mask.GetHeight();
                        if (maskImage.GetPixel((int)x, (int)y).r < 0.5f) continue;
                    }

                    positions.Add(finalPos);
                    break;
                }
            };

            if (maskImage != null) maskImage.Unlock();

            return positions;
        }

        private float SemiRandomFloat(float seed)
        {
            // Deterministically convert a float into another float from 0 to 1. Not very random but good enough for grass jittering
            var n = Mathf.Sin(seed * seed * 55.2325f) * seed * 0.4595f;
            n = Mathf.Abs(n);
            return n - (float)Math.Truncate(n);
        }

        private void GenerateGrassOnThread()
        {
            // Generate the grass on a thread separate from the main game loop, preventing lag spikes
            if (generateGrassThread == null || !generateGrassThread.IsAlive())
            {
                if (generateGrassThread != null) generateGrassThread.WaitToFinish();
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
            if (!generatedInitialGrass)
            {
                generatedInitialGrass = true;
                GenerateGrassOnThread();
            }
            else if (Engine.GetFramesDrawn() % 30 == 0 &&
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