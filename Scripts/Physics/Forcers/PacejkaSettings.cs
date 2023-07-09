using Godot;
using System;

namespace Physics.Forcers
{
    [Tool]
    public partial class PacejkaSettings : Resource
    {
        [Export] public float Peak { get; set; } = 1;
        [Export] public float Shape3D { get; set; } = 1.35f; // typical values: 1.35 for lateral and 1.65 for longitudinal
        [Export] public float Stiff { get; set; } = 10;
        [Export] public float Curve { get; set; } = 0;
    }
}