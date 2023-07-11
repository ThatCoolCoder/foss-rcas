using Godot;
using System;

namespace Aircraft;

public partial class FpvCamera : Locations.BasicFlightCamera
{
    private CanvasLayer overlay;

    public override void _Ready()
    {
        base._Ready();
        overlay = GetNode<CanvasLayer>("CanvasLayer");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        overlay.Visible = Current;
    }
}