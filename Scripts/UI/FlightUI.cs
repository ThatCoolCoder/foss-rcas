using Godot;
using System;

namespace Locations;

public partial class FlightUI : CanvasLayer
{
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("toggle_ui")) Visible = !Visible;
    }
}