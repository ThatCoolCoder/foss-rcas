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
                "Impostor shadows enabled",
                s => s.Graphics.ImpostorShadowsEnabled,
                (s, v) => s.Graphics.ImpostorShadowsEnabled = v,
                toolTip: "Whether to draw shadows for impostor trees");

            BooleanInput.Scene.Instance<BooleanInput>().Config(
                holder,
                "FPS counter enabled",
                s => s.Graphics.ShowFps,
                (s, v) => s.Graphics.ShowFps = v,
                toolTip: "Whether to show the frames per second on screen");

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Vegetation multiplier (near)",
                s => s.Graphics.NearVegetationMultiplier,
                (s, v) => s.Graphics.NearVegetationMultiplier = v,
                0, 3, step: .05f,
                toolTip: "Amount of nearby vegetation (trees & bushes)");

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Vegetation multiplier (far)",
                s => s.Graphics.FarVegetationMultiplier,
                (s, v) => s.Graphics.FarVegetationMultiplier = v,
                0, 3, step: .05f,
                toolTip: "Amount of far away vegetation(trees & bushes)");

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Grass multiplier",
                s => s.Graphics.GrassMultiplier,
                (s, v) => s.Graphics.GrassMultiplier = v,
                0, 3, step: .05f,
                toolTip: "Amount of grass");

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Grass distance multiplier",
                s => s.Graphics.GrassDistanceMultiplier,
                (s, v) => s.Graphics.GrassDistanceMultiplier = v,
                .1f, 3, step: .05f,
                toolTip: "Max distance for spawning grass");
        }

    }
}