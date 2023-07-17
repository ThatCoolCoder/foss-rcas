using Godot;
using System;


namespace UI.Settings;

using Components;

public partial class GraphicsTab : Control
{
    [Export] public Control Holder;
    [Export] public Control PresetHolder;

    public override void _Ready()
    {
        CreatePresetButtons();

        BooleanInput.Scene.Instantiate<BooleanInput>().Config(
            Holder,
            "Use impostors",
            s => s.Graphics.UseImpostors,
            (s, v) => s.Graphics.UseImpostors = v,
            toolTip: "Whether to use fake trees beyond a certain distance for better performance");

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Impostor distance",
            s => s.Graphics.ImpostorDistance,
            (s, v) => s.Graphics.ImpostorDistance = (int)v,
            0, 1000, step: 1, _formatString: "#",
            toolTip: "Distance at which to show impostors");

        BooleanInput.Scene.Instantiate<BooleanInput>().Config(
            Holder,
            "Impostor shadows enabled",
            s => s.Graphics.ImpostorShadowsEnabled,
            (s, v) => s.Graphics.ImpostorShadowsEnabled = v,
            toolTip: "Whether to draw shadows for impostor trees");

        BooleanInput.Scene.Instantiate<BooleanInput>().Config(
            Holder,
            "FPS counter enabled",
            s => s.Graphics.ShowFps,
            (s, v) => s.Graphics.ShowFps = v,
            toolTip: "Whether to show the frames per second on screen");

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Vegetation multiplier (near)",
            s => s.Graphics.NearVegetationMultiplier,
            (s, v) => s.Graphics.NearVegetationMultiplier = v,
            0, 3, step: .05f, _formatString: "0.00",
            toolTip: "Amount of nearby vegetation (trees & bushes)");

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Vegetation multiplier (far)",
            s => s.Graphics.FarVegetationMultiplier,
            (s, v) => s.Graphics.FarVegetationMultiplier = v,
            0, 3, step: .05f, _formatString: "0.00",
            toolTip: "Amount of far away vegetation (trees & bushes)");

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Grass multiplier",
            s => s.Graphics.GrassMultiplier,
            (s, v) => s.Graphics.GrassMultiplier = v,
            0, 3, step: .05f, _formatString: "0.00",
            toolTip: "Amount of grass");

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Grass distance multiplier",
            s => s.Graphics.GrassDistanceMultiplier,
            (s, v) => s.Graphics.GrassDistanceMultiplier = v,
            .1f, 3, step: .05f, _formatString: "0.00",
            toolTip: "Max distance for spawning grass");

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Directional shadow size",
            s => s.Graphics.DirectionalShadowSizeExponent,
            (s, v) => s.Graphics.DirectionalShadowSizeExponent = (int)v,
            8, 14, step: 1, _customDisplayFunc: v => (1 << (int)v).ToString());

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Shadow atlas size",
            s => s.Graphics.ShadowAtlasSizeExponent,
            (s, v) => s.Graphics.ShadowAtlasSizeExponent = (int)v,
            8, 14, step: 1, _customDisplayFunc: v => (1 << (int)v).ToString());

        NumericSliderInput.Scene.Instantiate<NumericSliderInput>().Config(
            Holder,
            "Shadow atlas cubemap size",
            s => s.Graphics.ShadowAtlasCubemapSizeExponent,
            (s, v) => s.Graphics.ShadowAtlasCubemapSizeExponent = (int)v,
            6, 14, step: 1, _customDisplayFunc: v => (1 << (int)v).ToString());

        EnumInput.Scene.Instantiate<EnumInput>().Config(
            Holder,
            "Anti-aliasing mode",
            s => s.Graphics.AntiAliasingMode,
            (s, v) => s.Graphics.AntiAliasingMode = (SimSettings.AntiAliasingMode)v,
            typeof(SimSettings.AntiAliasingMode));

        EnumInput.Scene.Instantiate<EnumInput>().Config(Holder,
            "Anti aliasing amount",
            s => s.Graphics.Msaa,
            (s, v) => s.Graphics.Msaa = (SubViewport.Msaa)v,
            typeof(SubViewport.Msaa),
            customValueFormatter: (obj) => obj.ToString().Replace("Msaa", ""),
            toolTip: "Amount of anti aliasing passes. Only applies if high quality anti aliasing is enabled");
    }

    private void CreatePresetButtons()
    {
        var presetButtonScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/GraphicsPresetButton.tscn");
        foreach (var preset in GraphicsPreset.Presets)
        {
            var button = presetButtonScene.Instantiate<GraphicsPresetButton>();
            button.GraphicsPreset = preset;
            button.OnClicked += ApplyPreset;
            PresetHolder.AddChild(button);
        }
    }

    private void ApplyPreset(GraphicsPreset preset)
    {
        var showFps = SettingsScreen.NewSettings.Graphics.ShowFps; // hacky way to persist this setting
        SettingsScreen.NewSettings.Graphics = GraphicsPreset.Clone(preset);
        SettingsScreen.NewSettings.Graphics.ShowFps = showFps;
        SettingsScreen.ChangeSettings();
    }

}