using Godot;
using System;

namespace Physics.Fluids;

public partial class WindSettings
{
    public float Speed { get; set; } = 0;
    public float GustSpeedDelta { get; set; } = 0;
    public float GustFrequency { get; set; } = 20;
    public float DirectionDegrees
    {
        get
        {
            return Mathf.RadToDeg(Direction);
        }
        set
        {
            Direction = Mathf.DegToRad(value);
        }
    }
    public float Direction = 3.14f;
    public float DirectionVariabilityDegrees // from 0 to 180
    {
        get
        {
            return Mathf.RadToDeg(DirectionVariability);
        }
        set
        {
            DirectionVariability = Mathf.DegToRad(value);
        }
    }
    public float DirectionVariability = 0.4f;
    public float DirectionChangeFrequency = 30;

    public float TurbulenceMaxSpeed { get; set; } = 0; // (horizontal turbulence)
    public float WindshearMaxSpeed { get; set; } = 0; // i.e vertical turbulence
    public float TurbulenceFrequency { get; set; } = 1;

    public static WindSettings SlightGusts = new()
    {
        TurbulenceMaxSpeed = 2,
        WindshearMaxSpeed = .5f,
        TurbulenceFrequency = 1,
    };
}