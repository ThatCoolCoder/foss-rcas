using Godot;
using System;

namespace Aircraft
{
    public class FpvCamera : Locations.FlightCamera
    {
        private CanvasLayer overlay;

        public override void _Ready()
        {
            base._Ready();
            overlay = GetNode<CanvasLayer>("CanvasLayer");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            overlay.Visible = Current;
        }
    }
}