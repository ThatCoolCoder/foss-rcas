using Godot;
using System;

namespace UI.Settings.Components
{
    public class AxisEditor : Control
    {
        public override void _Ready()
        {
            GetNode<BooleanInput>("InvertedInput").Config(null, "Inverted", s => false, (s, v) => { var a = 5; });
        }
    }
}
