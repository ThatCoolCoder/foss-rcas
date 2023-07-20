using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers;

public partial class BuoyancyForcer : AbstractSpatialFluidForcer
{
    // Thing that calculates buoyant force in a bounded fluid (preferably a liquid)

    // Do not change while simulation is going, must only set it at the start.
    // Curve from 0 to 1 that mimics the side profile of the hull, allowing us to approximate tapered/curved hulls without doing full mesh-based volume calculation
    [Export] public Curve HullShapeCurve { get; set; }
    private Curve depthToAreaCurve = null;
    private const int numDepthSampleValues = 20;

    protected override bool autoCheckInsideFluid { get; set; } = false;

    public override void _Ready()
    {
        UpdateDebugBoxVisibility();
        onDebugModeChanged += UpdateDebugBoxVisibility;
        if (HullShapeCurve != null) CalculateDepthToAreaCurve();
        base._Ready();
    }

    public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState3D state)
    {
        if (!fluid.HasBoundaries) return Vector3.Zero;

        var waterLevel = fluid.BoundaryAtPoint(GlobalPosition).Y;
        var waterDensity = fluid.DensityAtPoint(GlobalPosition);

        var top = GlobalPosition.Y + Scale.Y / 2;
        var bottom = GlobalPosition.Y - Scale.Y / 2;

        float immersion = 1;
        float volume = Scale.X * Scale.Y * Scale.Z;
        var buoyancyDirection = fluid.NormalAtPoint(GlobalPosition);


        if (top > waterLevel)
        {
            immersion = Mathf.Max(0, waterLevel - bottom) / Scale.Y;
            if (depthToAreaCurve != null) immersion = depthToAreaCurve.Sample(immersion);
        }

        volume *= immersion;

        var buoyantForce = buoyancyDirection * volume * waterDensity * (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

        return buoyantForce;

    }

    private void CalculateDepthToAreaCurve()
    {
        // convert hull shape curve to area curve

        depthToAreaCurve = new Curve();
        float runningTotal = 0;
        float interval = 1.0f / (float)numDepthSampleValues;
        for (int i = 0; i < numDepthSampleValues + 1; i++)
        {
            float xPos = interval * i;
            runningTotal += HullShapeCurve.Sample(xPos) / numDepthSampleValues;
            depthToAreaCurve.AddPoint(new Vector2(xPos, runningTotal), leftMode: Curve.TangentMode.Linear, rightMode: Curve.TangentMode.Linear);
        }
    }


    private void UpdateDebugBoxVisibility()
    {
        GetNode<Node3D>("DebugBox").Visible = DebugModeActive;
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint()) return;

        onDebugModeChanged -= UpdateDebugBoxVisibility;
    }
}
