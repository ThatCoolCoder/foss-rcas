using Godot;
using System;
using System.Collections.Generic;


namespace Locations;

public partial class GrassScatter : MultiMeshInstance3D
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
    [Export] public Texture2D Mask { get; set; } // Only on white regions of this texture is grass spawned. If you leave it out then it's just everywhere
    [Export] public Texture2D Texture2D { get; set; }
    [Export] public Texture2D NormalMap { get; set; }
    [Export] public float NormalStrength { get; set; } = 1;
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
    private GodotThread generateGrassThread;
    private Vector3 crntCameraPos;
    private Vector3 lastUpdatePos;
    private bool generatedInitialGrass = false;

    public override void _Ready()
    {
        // Clean up the editor visualisations
        GetNode<Node3D>("CSGBox3D").Visible = false;

        size = Scale;
        GetNode<Node3D>("CSGBox3D").Scale = Scale;
        Scale = Vector3.One;
    }

    private void GenerateGrass()
    {
        // todo: probably we could move some of this code to ready()
        MultiMesh multimesh = new();
        multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        var mesh = new QuadMesh();
        mesh.Size = GrassSize;
        multimesh.Mesh = mesh;
        multimesh.UseColors = true;

        var material = new StandardMaterial3D();
        material.AlbedoTexture = Texture2D;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        mesh.Material = material;
        material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;

        material.DistanceFadeMaxDistance = 0;
        material.DistanceFadeMinDistance = trueFalloffMaxDistance;
        material.DistanceFadeMode = StandardMaterial3D.DistanceFadeModeEnum.PixelAlpha;

        if (NormalMap == null) material.NormalEnabled = false;
        else
        {
            material.NormalEnabled = true;
            material.NormalTexture = NormalMap;
            material.NormalScale = NormalStrength;
        }

        var cameraPos = crntCameraPos;
        lastUpdatePos = cameraPos;
        var positions = GenerateGrassPositionsV3(cameraPos);

        multimesh.InstanceCount = positions.Count;

        for (int i = 0; i < positions.Count; i++)
        {
            // Calculcate rotation + scale
            var transform = Transform3D.Identity;
            var pos = positions[i];

            Func<float, float, float, float> getAxisScale =
                (float x, float z, float variation) => 1 + (SemiRandomFloat(x + z * 1.52f) - .5f) * 2 * variation;
            transform = transform.Scaled(new Vector3(
                getAxisScale(pos.X, pos.Z, GrassSizeVariation.X),
                getAxisScale(pos.X, pos.Z, GrassSizeVariation.Y),
                getAxisScale(pos.X, pos.Z, GrassSizeVariation.X)
            ));

            transform = transform.Rotated(Vector3.Up, SemiRandomFloat(pos.X + pos.Z) * Mathf.Tau);

            // Adjust position then save
            transform.Origin = pos.WithY(transform.Basis.Scale.Y * GrassSize.Y / 2);
            multimesh.SetInstanceTransform(i, transform);
            multimesh.SetInstanceColor(i, Colors.Red);
        }

        Multimesh = multimesh;
    }

    private List<Vector3> GenerateGrassPositionsV1(Vector3 falloffCenter)
    {
        // Create the positions for the grass
        // the blades may need to be moved up or down depending on their height

        // Most basic algorithm, just places them in a circle around the camera

        int targetInstanceCount = trueInstanceCount;

        var maskImage = Mask == null ? null : Mask.GetImage();
        // if (maskImage != null) maskImage.Lock();

        var positions = new List<Vector3>();

        for (int i = 0; i < targetInstanceCount; i++)
        {
            for (int attemptNum = 0; attemptNum < MaxMaskTries; attemptNum++)
            {
                var distMultiplier = GD.Randf();
                var relativeToCamera = Vector3.Forward.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau) * distMultiplier * trueFalloffMaxDistance;
                var pos = ToLocal(relativeToCamera + falloffCenter);

                // Check if within bounds
                if (Mathf.Abs(pos.X) > (size.X / 2) || Mathf.Abs(pos.Z) > (size.Z / 2)) continue;

                // Check if within mask
                if (Mask != null)
                {
                    var x = (pos.X / size.X + 0.5f) * Mask.GetWidth();
                    var y = (pos.Z / size.Z + 0.5f) * Mask.GetHeight();
                    if (maskImage.GetPixel((int)x, (int)y).R < 0.5f) continue;
                }

                positions.Add(pos);
                break;
            }
        };

        // var culledPositions = 

        // if (maskImage != null) maskImage.Unlock();

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

        var maskImage = Mask == null ? null : Mask.GetImage();
        // if (maskImage != null) maskImage.Lock();

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
                if (Mathf.Abs(pos.X) > (size.X / 2) || Mathf.Abs(pos.Z) > (size.Z / 2)) continue;

                // Check if within mask
                if (Mask != null)
                {
                    var x = (pos.X / size.X + 0.5f) * Mask.GetWidth();
                    var y = (pos.Z / size.Z + 0.5f) * Mask.GetHeight();
                    if (maskImage.GetPixel((int)x, (int)y).R < 0.5f) continue;
                }

                initialPositions.Add(pos);
                break;
            }
        };

        // if (maskImage != null) maskImage.Unlock();
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

        var maskImage = Mask == null ? null : Mask.GetImage();
        // if (maskImage != null) maskImage.Lock();

        var positions = new List<Vector3>();

        for (int i = 0; i < targetInstanceCount; i++)
        {
            for (int attemptNum = 0; attemptNum < MaxMaskTries; attemptNum++)
            {
                var distMultiplier = SemiRandomFloat(i * 2.235f + attemptNum);
                var relativeToCamera = Vector3.Forward.Rotated(Vector3.Up, SemiRandomFloat(i * 4.554f + attemptNum) * Mathf.Tau) * distMultiplier * trueFalloffMaxDistance;
                var pos = ToLocal(relativeToCamera + falloffCenter);

                // Snap to grid
                var snappedPos = new Vector3(Utils.RoundTo(pos.X, gridSpacing), 0, Utils.RoundTo(pos.Z, gridSpacing));

                // Jiggle
                var jiggle = new Vector3(
                    SemiRandomFloat(pos.X + 3 * pos.Z) * gridSpacing,
                    0,
                    SemiRandomFloat(3 * pos.X + pos.Z) * gridSpacing);
                var finalPos = snappedPos + jiggle;

                // Check if within bounds
                if (Mathf.Abs(finalPos.X) > (size.X / 2) || Mathf.Abs(finalPos.Z) > (size.Z / 2)) continue;

                // Check if within mask
                if (Mask != null)
                {
                    var x = (finalPos.X / size.X + 0.5f) * Mask.GetWidth();
                    var y = (finalPos.Z / size.Z + 0.5f) * Mask.GetHeight();
                    if (maskImage.GetPixel((int)x, (int)y).R < 0.5f) continue;
                }

                positions.Add(finalPos);
                break;
            }
        };

        // if (maskImage != null) maskImage.Unlock();

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
        // if (generateGrassThread == null || !generateGrassThread.IsAlive())
        // {
        //     if (generateGrassThread != null) generateGrassThread.WaitToFinish();
        crntCameraPos = GetViewport().GetCamera3D().GlobalPosition;
        GenerateGrass();
        //     generateGrassThread = new GodotThread();
        //     // generateGrassThread.Start(new Callable(this, MethodName.GenerateGrass)); // convtodo: fix threading and enable crass
        // }
    }

    private Vector3 GetCameraPos()
    {
        return GetViewport().GetCamera3D().GlobalPosition;
    }

    public override void _Process(double delta)
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