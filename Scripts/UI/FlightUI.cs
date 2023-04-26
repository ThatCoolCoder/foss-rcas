using Godot;
using System;

namespace Locations
{
    public class FlightUI : CanvasLayer
    {
        public override void _Process(float delta)
        {
            if (Input.IsActionJustPressed("toggle_ui")) Visible = !Visible;
        }
    }
}