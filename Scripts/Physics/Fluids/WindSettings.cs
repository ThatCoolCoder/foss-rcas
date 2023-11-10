using Godot;
using System;

namespace Physics.Fluids;

public partial class WindSettings
{
    public float Speed { get; set; } = 0;
    public float GustSpeedDelta { get; set; } = 0; // max speed = speed + this
    public float GustFrequency { get; set; } = 1;
    public float DirectionDegrees = 0;
    public float DirectionVariabilityDegrees = 30; // final dir = dir plus minus this
    public float DirectionChangeFrequency = 1;

    public float TurbulenceMaxSpeed { get; set; } = 0; // (horizontal turbulence)
    public float WindshearMaxSpeed { get; set; } = 0; // i.e vertical turbulence
    public float TurbulenceFrequency { get; set; } = 1;

    public static WindSettings SlightGusts = new()
    {
        TurbulenceMaxSpeed = 2,
        WindshearMaxSpeed = .5f,
        TurbulenceFrequency = 1,
    };

    public static WindSettings NorthBreeze = new()
    {
        Speed = 3,
        GustSpeedDelta = 1,
        DirectionDegrees = 0,
        DirectionVariabilityDegrees = 0,

        TurbulenceMaxSpeed = 0,
        WindshearMaxSpeed = 0,
    };
}