using Godot;
using System;

namespace Physics.Forcers
{
    [Tool]
    public class AeroCoefficientCube : Resource
    {
        // Lift cube or drag cube.
        // Values refer to the direction that the object is moving in - if it's going to the right
        // If not travelling on an orthogonal direction, then it's interpolated between them.

        [Export] public float Up { get; set; } = 1;
        [Export] public float Down { get; set; } = 1;
        [Export] public float Left { get; set; } = 1;
        [Export] public float Right { get; set; } = 1;
        [Export] public float Forward { get; set; } = 1;
        [Export] public float Back { get; set; } = 1;
    }
}