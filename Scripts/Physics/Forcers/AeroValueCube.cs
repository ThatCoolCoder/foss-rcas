using Godot;
using System;

namespace Physics.Forcers;

[Tool]
[GlobalClass]
public partial class AeroValueCube : Resource
{
    // Lift cube, drag cube, etc
    // Values refer to the direction that the object is moving in - if it's going to the right then Right is used
    // If not travelling on an orthogonal direction, then it's interpolated between them.

    [Export] public float Left { get; set; } = 1;
    [Export] public float Right { get; set; } = 1;
    [Export] public float Up { get; set; } = 1;
    [Export] public float Down { get; set; } = 1;
    [Export] public float Forward { get; set; } = 1;
    [Export] public float Back { get; set; } = 1;

    public static AeroValueCube FromVector3(Vector3 value)
    {
        // Create an AeroValueCube from a Vector3 in which x corresponds to left/right,
        // y corresponds to up/down and z corresponds to forward/back

        return new AeroValueCube()
        {
            Left = Mathf.Abs(value.X),
            Right = Mathf.Abs(value.X),
            Up = Mathf.Abs(value.Y),
            Down = Mathf.Abs(value.Y),
            Forward = Mathf.Abs(value.Z),
            Back = Mathf.Abs(value.Z),
        };
    }
}