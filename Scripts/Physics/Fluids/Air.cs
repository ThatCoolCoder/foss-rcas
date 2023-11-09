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

    private FastNoiseLite turbulenceNoise = new(); // why we need 3 noise when we can just use 1 and offset sample?
    private float time = 0;

    public override void _Ready()
    {
        turbulenceNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Value;
        turbulenceNoise.FractalOctaves = 1;
    }

    public override void _PhysicsProcess(double delta)
    {
        time += (float)delta;
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
        // todo: add base flow as well as flow field
        var flow = Vector3.Zero;

        // Define these for setting sensible base values for the frequencies to be based on
        var baseTime = time * 100f;
        var basePos = point * 10f;

        var turbulenceSpeed = Utils.MapNumber(turbulenceNoise.GetNoise3D(basePos.X, basePos.Y, baseTime * WindSettings.TurbulenceFrequency), -1, 1, 0, WindSettings.TurbulenceMaxSpeed);
        // Mapping direction from 0 to 4 rotations in an effort to make it not biased to center
        var turbulenceDirection = Utils.MapNumber(turbulenceNoise.GetNoise3D(basePos.X, basePos.Y + 1000, baseTime * .5f * WindSettings.TurbulenceFrequency), -1, 1, 0, Mathf.Pi * 2 * 4);

        var windshearSpeed = Utils.MapNumber(turbulenceNoise.GetNoise3D(basePos.X, basePos.Y + 1100, baseTime * WindSettings.TurbulenceFrequency), -1, 1,
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