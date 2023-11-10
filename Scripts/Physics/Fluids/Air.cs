using Godot;
using System;

namespace Physics.Fluids;

[GlobalClass]
[Tool]
public partial class Air : Node3D, ISpatialFluid
{
    // Air, supports basic wind with gusting and direction changing

    [Export] public float DensityMultiplier { get; set; } = 1;

    public WindSettings WindSettings { get; set; } = WindSettings.SlightGusts;

    private FastNoiseLite mainNoise = new();
    private FastNoiseLite turbulenceNoise = new(); // why we need 3 noise when we can just use 1 and offset sample?
    private float time = 0;

    private Vector3 cachedMainFlow;

    public override void _Ready()
    {
        turbulenceNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Value;
        turbulenceNoise.FractalOctaves = 1;

        mainNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Value;
        mainNoise.FractalOctaves = 2;
    }

    public override void _PhysicsProcess(double delta)
    {
        time += (float)delta;

        var mainFlowDirectionDegrees = WindSettings.DirectionDegrees;
        mainFlowDirectionDegrees += mainNoise.GetNoise1D(time * 10 * WindSettings.DirectionChangeFrequency) * WindSettings.DirectionVariabilityDegrees;
        var mainFlowDirection = Mathf.DegToRad(mainFlowDirectionDegrees);
        var mainFlowSpeed = WindSettings.Speed + Utils.MapNumber(mainNoise.GetNoise1D(time * 10 * WindSettings.GustFrequency + 2502.2f), -1, 1, 0, WindSettings.GustSpeedDelta);
        cachedMainFlow = Vector3.Right.Rotated(Vector3.Up, mainFlowDirection) * mainFlowSpeed;
    }

    public override void _Process(double delta)
    {
        // GD.Print(VelocityAtPoint(Vector3.Zero));
    }

    public float DensityAtPoint(Vector3 _point)
    {
        return 1.293f * DensityMultiplier;
    }

    public bool ContainsPoint(Vector3 _point)
    {
        return true; // air is everywhere, except underground or underwater, but air is so much less dense than those places that it doesn't matter
    }

    public Vector3 VelocityAtPoint(Vector3 point)
    {
        var flow = cachedMainFlow;

        // Define this for setting sensible base values for the frequencies to be based on
        var basePos = point * 10f;

        var turbulenceSpeed = Utils.MapNumber(turbulenceNoise.GetNoise3D(basePos.X, basePos.Y, time * 100 * WindSettings.TurbulenceFrequency), -1, 1, 0, WindSettings.TurbulenceMaxSpeed);
        // Mapping direction from 0 to 4 rotations in an effort to make it not biased to center
        var turbulenceDirection = Utils.MapNumber(turbulenceNoise.GetNoise3D(basePos.X, basePos.Y + 1000, time * 5 * WindSettings.TurbulenceFrequency), -1, 1, 0, Mathf.Pi * 2 * 4);

        var windshearSpeed = Utils.MapNumber(turbulenceNoise.GetNoise3D(basePos.X, basePos.Y + 1100, time * 100 * WindSettings.TurbulenceFrequency), -1, 1,
            -WindSettings.WindshearMaxSpeed, WindSettings.WindshearMaxSpeed);

        flow += new Vector3(Mathf.Cos(turbulenceDirection) * turbulenceSpeed, windshearSpeed, Mathf.Sin(turbulenceDirection) * turbulenceSpeed);

        return flow;
    }

    public Vector3 NormalAtPoint(Vector3 _point)
    {
        return Vector3.Up;
    }

    public Vector3 BoundaryAtPoint(Vector3 point)
    {
        throw new NotImplementedException();
    }

    public bool HasBoundaries { get; set; } = false;
}