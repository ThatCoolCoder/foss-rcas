using Godot;
using System;


namespace UI.Settings
{
    using Components;

    public class GraphicsTab : Control
    {

        public override void _Ready()
        {
            var holder = GetNode<Control>("MaxSizeContainer/VBoxContainer");

            BooleanInput.Scene.Instance<BooleanInput>().Config(
                holder,
                "Use impostors",
                s => s.Graphics.UseImpostors,
                (s, v) => s.Graphics.UseImpostors = v,
                toolTip: "Whether to use impostor trees beyond a certain distance for better performance");

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Impostor distance",
                s => s.Graphics.ImpostorDistance,
                (s, v) => s.Graphics.ImpostorDistance = (int)v,
                0, 1000, step: 1,
                toolTip: "Distance at which to show impostors");

            BooleanInput.Scene.Instance<BooleanInput>().Config(
                holder,
                "FPS counter enabled",
                s => s.Graphics.ShowFps,
                (s, v) => s.Graphics.ShowFps = v,
                toolTip: "Whether to show the frames per second on screen");

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Vegetation multiplier",
                s => s.Graphics.VegetationMultiplier,
                (s, v) => s.Graphics.VegetationMultiplier = v,
                0, 1, step: 0.1f,
                toolTip: "Amount of vegetation (trees & grass)");
        }

    }
}