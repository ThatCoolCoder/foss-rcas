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
    [Export] public float CameraMoveDistBeforeUpdate { get; set; } = 10; // Update grass falloff when camera moves this far
    [Export] public int InstanceCount { get; set; } = 100;
    [Export] public Texture2D Mask { get; set; } // Only on white regions of this texture is grass spawned. If you leave it out then it's just everywhere
    [Export] public Texture2D Texture2D { get; set; }
    [Export] public Texture2D NormalMap { get; set; }
    [Export] public float NormalStrength { get; set; } = 1;
    [Export] public Vector2 GrassSize { get; set; } = new Vector2(0.07f, 0.5f);
    [Export] public Vector2 GrassSizeVariation { get; set; } = Vector2.One * 0.3f; // varies by +- this amount
    [Export] public Node3D HTerrain { get; set; } = null;

    // If we fail to find a position that satisfies the mask in this many tries, give up.
    // Try reducing this if grass takes too long to generate
    [Export] public int MaxMaskTries { get; set; } = 10;

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
        GodotThread.SetThreadSafetyChecksEnabled(false);
        // todo: probably we could move some of this code to ready()
        MultiMesh multimesh = new();
        multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        var mesh = new QuadMesh();
        mesh.Size = GrassSize;
        multimesh.Mesh = mesh;
        multimesh.UseColors = true;

        var material = new StandardMaterial3D();
        material.AlbedoTexture = Texture2D;
        material.Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor;
        mesh.Material = material;
        material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;

        material.DistanceFadeMaxDistance = Mathf.Sqrt(trueFalloffMaxDistance);
        material.DistanceFadeMinDistance = trueFalloffMaxDistance;
        material.DistanceFadeMode = StandardMaterial3D.DistanceFadeModeEnum.PixelDither;

        if (NormalMap == null) material.NormalEnabled = false;
        else
        {
            material.NormalEnabled = true;
            material.NormalTexture = NormalMap;
            material.NormalScale = NormalStrength;
        }

        var cameraPos = crntCameraPos;
        lastUpdatePos = cameraPos;
        var positions = GenerateGrassPositionsV4(cameraPos);

        multimesh.InstanceCount = positions.Count;

        var hTerrainData = (Resource)HTerrain?.Call("get_data");
        var image = (Image)hTerrainData?.Call("get_image", 0);
        var heightMapSize = (Vector2I)image?.GetSize();
        var mapScale = (Vector3)HTerrain?.Get("map_scale");
        var mapCentered = (bool)HTerrain?.Get("centered");

        GD.Print(Utils.GetHeightFromHTerrainInterpolated(Vector3.Zero, mapScale, image, heightMapSize, mapCentered));

        for (int i = 0; i < positions.Count; i++)
        {
            // Calculate rotation + scale
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
            var yPos = transform.Basis.Scale.Y * GrassSize.Y / 2;
            if (hTerrainData != null)
            {
                yPos += Utils.GetHeightFromHTerrainInterpolated(pos, mapScale, image, heightMapSize, mapCentered);
                // var s = image.GetSize() / 2;
                // var p = new Vector3(pos.X / mapScale.X + s.X, 0, pos.Z / mapScale.Z + s.Y);
                // yPos += (float)hTerrainData.Call("get_interpolated_height_at", p);
            }

            transform.Origin = pos.WithY(yPos);
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

        return positions;
    }

    private List<Vector3> GenerateGrassPositionsV4(Vector3 center)
    {
        // Make square grid around camera
        // jiggle points deterministically
        // check mask and bounds
        // done.

        // By having a consistent density, we can counterinituitively have a much higher blade count,
        // because it turns out that the intersecting blades in the center of the other places are what slowed it down.

        // Don't even bother trying if we're far enough away
        var maxPos = size / 2 + trueFalloffMaxDistance * Vector3.One;
        if (Mathf.Abs(center.X) > maxPos.X || Mathf.Abs(center.Z) > maxPos.Z) return new();

        var g = SimSettings.Settings.Current.Graphics;
        int targetInstanceCount = (int)(InstanceCount * g.GrassMultiplier * g.GrassDistanceMultiplier * g.GrassDistanceMultiplier);
        var gridSize = Mathf.Sqrt(targetInstanceCount);
        var gridSpacing = 2 * trueFalloffMaxDistance / gridSize;

        var offset = ToLocal(center) - new Vector3(trueFalloffMaxDistance, 0, trueFalloffMaxDistance);

        var maskImage = Mask == null ? null : Mask.GetImage();

        var positions = new List<Vector3>();

        for (int i = 0; i < targetInstanceCount; i++)
        {
            for (int attemptNum = 0; attemptNum < MaxMaskTries; attemptNum++)
            {
                var pos = offset + new Vector3(i % gridSize, 0, i / gridSize) * gridSpacing;

                // Snap to grid
                pos = new Vector3(Utils.RoundTo(pos.X, gridSpacing), 0, Utils.RoundTo(pos.Z, gridSpacing));

                // Jiggle
                var jiggle = new Vector3(
                    SemiRandomFloat(pos.X + 3 * pos.Z) * gridSpacing,
                    0,
                    SemiRandomFloat(3 * pos.X + pos.Z) * gridSpacing);
                var finalPos = pos + jiggle;

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
            crntCameraPos = GetViewport().GetCamera3D().GlobalPosition;
            generateGrassThread = new GodotThread();
            generateGrassThread.Start(new Callable(this, MethodName.GenerateGrass));
        }
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