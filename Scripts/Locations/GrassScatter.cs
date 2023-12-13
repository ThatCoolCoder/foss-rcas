using Godot;
using System;
using System.Collections.Generic;


namespace Locations;

[Tool]
public partial class GrassScatter : Node3D
{
    // Thing that scatters grass within a rectangular region while also respecting a mask and only creating it within a certain distance of the camera.
    // Updates the grass when camera moves but tries not to jitter existing blades
    // Configure the rectangular region by scaling this.
    // Probably is a dumb way to do things but I can't think of anything better
    // Potential improvement: make it a tool script and make the mask editable in the editor.
    // todo: Is a bit hacky, could do with some refactoring

    [Export] public Vector2 Size { get; set; } = Vector2.One * 100;
    [Export] public float FalloffMaxDistance { get; set; } = 100;
    [Export] public float CameraMoveDistBeforeUpdate { get; set; } = 10; // Update grass falloff when camera moves this far
    [Export] public int InstanceCount { get; set; } = 100;
    [Export] public Texture2D Mask { get; set; } // Only on white regions of this texture is grass spawned. If you leave it out then it's just everywhere
    [Export] public Texture2D Texture { get; set; }
    [Export] public float BrightnessAdjust { get; set; } = 1;
    [Export] public Color ColorAdjust { get; set; } = Colors.Black;
    [Export] public Texture2D NormalMap { get; set; }
    [Export] public float NormalStrength { get; set; } = 1;
    [Export] public Vector2 GrassSize { get; set; } = new Vector2(0.07f, 0.5f);
    [Export] public Vector2 GrassSizeVariation { get; set; } = Vector2.One * 0.3f; // varies by +- this amount
    [Export] public Node3D HTerrain { get; set; } = null;
    [Export] public Physics.Fluids.Air Air { get; set; } = null;

    // If we fail to find a position that satisfies the mask in this many tries, give up.
    // Try reducing this if grass takes too long to generate
    [Export] public int MaxMaskTries { get; set; } = 10;
    private CsgBox3D csgBox;
    private MultiMeshInstance3D mmi;

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


    private GodotThread generateGrassThread;
    private Vector3 crntCameraPos;
    private Vector3 lastUpdatePos;
    private bool generatedInitialGrass = false;
    private readonly Shader shader = ResourceLoader.Load<Shader>("Resources/Grass.gdshader");

    public override void _Ready()
    {
        // Clean up the editor visualisations
        csgBox = GetNode<CsgBox3D>("CSGBox3D");

        if (!Engine.IsEditorHint()) csgBox.Visible = false;

        Scale = Vector3.One;
        mmi = new MultiMeshInstance3D();
        AddChild(mmi);
        mmi.Owner = null;
    }

    private void GenerateGrass()
    {
        GodotThread.SetThreadSafetyChecksEnabled(false);
        // todo: probably we could move some of this code to ready()
        MultiMesh multimesh = new();
        multimesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;

        var mesh = new QuadMesh();
        multimesh.Mesh = mesh;
        mesh.Size = GrassSize;
        multimesh.UseColors = true;

        var material = new ShaderMaterial();
        mesh.Material = material;
        material.Shader = shader;

        material.SetShaderParameter("albedo", Texture);
        material.SetShaderParameter("bright_adjust", BrightnessAdjust);
        material.SetShaderParameter("color_adjust", ColorAdjust);
        material.SetShaderParameter("normal", NormalMap);
        material.SetShaderParameter("use_normal", NormalMap != null);
        material.SetShaderParameter("normal_strength", NormalStrength);
        material.SetShaderParameter("falloff_max_distance", trueFalloffMaxDistance);

        var cameraPos = crntCameraPos;
        lastUpdatePos = cameraPos;
        var positions = GenerateGrassPositions(cameraPos);

        multimesh.InstanceCount = positions.Count;

        var hTerrainData = (Resource)HTerrain?.Call("get_data");
        var image = (Image)hTerrainData?.Call("get_image", 0);
        var heightMapSize = image?.GetSize();
        var mapScale = HTerrain?.Get("map_scale");
        var mapCentered = HTerrain?.Get("centered");

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
                yPos += Utils.GetHeightFromHTerrainInterpolated(pos, (Vector3)mapScale, image, (Vector2I)heightMapSize, (bool)mapCentered);
            }

            transform.Origin = pos.WithY(yPos);
            multimesh.SetInstanceTransform(i, transform);
            multimesh.SetInstanceColor(i, Colors.Red);
        }

        mmi.Multimesh = multimesh;
    }
    private List<Vector3> GenerateGrassPositions(Vector3 center)
    {
        // Make square grid around camera
        // jiggle points deterministically
        // check mask and bounds
        // done.

        // By having a consistent density, we can counterinituitively have a much higher blade count,
        // because it turns out that the intersecting blades in the center of the other places are what slowed it down.

        // Don't even bother trying if we're far enough away
        var maxPos = Size / 2 + trueFalloffMaxDistance * Vector2.One;
        if (Mathf.Abs(center.X) > maxPos.X || Mathf.Abs(center.Z) > maxPos.Y) return new();

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
                if (Mathf.Abs(finalPos.X) > (Size.X / 2) || Mathf.Abs(finalPos.Z) > (Size.Y / 2)) continue;

                // Check if within mask
                if (Mask != null)
                {
                    var x = (finalPos.X / Size.X + 0.5f) * Mask.GetWidth();
                    var y = (finalPos.Z / Size.Y + 0.5f) * Mask.GetHeight();
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
        else
        {

            if (Engine.IsEditorHint())
            {
                // todo: when godot 4.2 releases, use the new editor-camera-position-getter
                if (Engine.GetFramesDrawn() % 120 == 0)
                {
                    GenerateGrassOnThread();
                }
                csgBox.Size = new Vector3(Size.X, 0.1f, Size.Y);
            }

            else if (Engine.GetFramesDrawn() % 30 == 0 &&
                GetCameraPos().DistanceSquaredTo(lastUpdatePos) > CameraMoveDistBeforeUpdate * CameraMoveDistBeforeUpdate)
            {
                GenerateGrassOnThread();
            }
        }
        UpdateWindInShader();
    }

    public override void _ExitTree()
    {
        if (generateGrassThread != null) generateGrassThread.WaitToFinish();
    }

    private void UpdateWindInShader()
    {
        // Yes we could just calc the wind in the shader. But I'm lazy and 100x100 samples should be enough.
        // Todo: make size sampled adjustable

        if (Air == null || trueInstanceCount == 0 || Engine.IsEditorHint()) return;

        var material = (mmi.Multimesh.Mesh as QuadMesh).Material as ShaderMaterial;

        var numSamples = 100;
        float size = 50;
        var corner = crntCameraPos - new Vector3(size / 2, 0, size / 2);

        var windVelocityMap = Image.Create(numSamples, numSamples, false, Image.Format.Rgf);

        var interval = size / (float)numSamples;

        for (int xS = 0; xS < numSamples; xS++) // xS as in x sample number
        {
            for (int zS = 0; zS < numSamples; zS++)
            {
                var posGlobal = new Vector3(corner.X + xS * interval, 0, corner.Z + zS * interval);
                posGlobal.Y = 0;
                var wind = Air.VelocityAtPoint(posGlobal);
                windVelocityMap.SetPixel(xS, zS, new Color(wind.X, wind.Z, 0));
            }
        }

        material.SetShaderParameter("wind_velocity_map", ImageTexture.CreateFromImage(windVelocityMap));
        material.SetShaderParameter("wind_velocity_map_corner", corner);
        material.SetShaderParameter("wind_velocity_map_size", size);
    }
}