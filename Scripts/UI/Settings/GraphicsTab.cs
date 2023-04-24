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

            CreatePresetButtons();

            BooleanInput.Scene.Instance<BooleanInput>().Config(
                holder,
                "Use impostors",
                s => s.Graphics.UseImpostors,
                (s, v) => s.Graphics.UseImpostors = v,
                toolTip: "Whether to use fake trees beyond a certain distance for better performance");

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
                toolTip: "Amount of far away vegetation (trees & bushes)");

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

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Directional shadow size",
                s => s.Graphics.DirectionalShadowSizeExponent,
                (s, v) => s.Graphics.DirectionalShadowSizeExponent = (int) v,
                8, 14, step: 1, _customDisplayFunc: v => 1 << (int) v);

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Shadow atlas size",
                s => s.Graphics.ShadowAtlasSizeExponent,
                (s, v) => s.Graphics.ShadowAtlasSizeExponent = (int) v,
                8, 14, step: 1, _customDisplayFunc: v => 1 << (int) v);

            NumericSliderInput.Scene.Instance<NumericSliderInput>().Config(
                holder,
                "Shadow atlas cubemap size",
                s => s.Graphics.ShadowAtlasCubemapSizeExponent,
                (s, v) => s.Graphics.ShadowAtlasCubemapSizeExponent = (int) v,
                6, 14, step: 1, _customDisplayFunc: v => 1 << (int) v);

            EnumInput.Scene.Instance<EnumInput>().Config(
                holder,
                "Anti-aliasing mode",
                s => s.Graphics.AntiAliasingMode,
                (s, v) => s.Graphics.AntiAliasingMode = (SimSettings.AntiAliasingMode) v,
                typeof(SimSettings.AntiAliasingMode));

            EnumInput.Scene.Instance<EnumInput>().Config(holder,
                "Anti aliasing amount",
                s => s.Graphics.Msaa,
                (s, v) => s.Graphics.Msaa = (Viewport.MSAA) v,
                typeof(Viewport.MSAA),
                customValueFormatter: (obj) => obj.ToString().Replace("Msaa", ""),
                toolTip: "Amount of anti aliasing passes. Only applies if high quality anti aliasing is enabled");
        }

        private void CreatePresetButtons()
        {
            var presetButtonScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/GraphicsPresetButton.tscn");
            var holder = GetNode<Control>("MaxSizeContainer/VBoxContainer/Presets");
            foreach (var preset in GraphicsPreset.Presets)
            {
                var button = presetButtonScene.Instance<GraphicsPresetButton>();
                button.GraphicsPreset = preset;
                button.OnClicked += ApplyPreset;
                holder.AddChild(button);
            }
        }

        private void ApplyPreset(GraphicsPreset preset)
        {
            SettingsScreen.NewSettings.Graphics = GraphicsPreset.Clone(preset);
            SettingsScreen.ChangeSettings();
        }

    }
}