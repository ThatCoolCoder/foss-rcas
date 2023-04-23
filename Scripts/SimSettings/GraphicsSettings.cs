using Godot;
using System;
using Tomlet;

namespace SimSettings
{
    public class GraphicsSettings
    {
        public bool ShowFps { get; set; } = false;
        public bool UseImpostors { get; set; } = true;
        public int ImpostorDistance { get; set; } = 50;
        public bool ImpostorShadowsEnabled { get; set; } = true;

        // A note on these multipliers:
        // Locations should be created such that when these multipliers equal 1,
        // a computer with a mid range discrete gpu (rtx 3050 rx6600) can achieve the target fps
        // todo: put this info in the location-creation documentation when that is made
        public float FarVegetationMultiplier { get; set; } = 1;
        public float NearVegetationMultiplier { get; set; } = 1;
        public float GrassMultiplier { get; set; } = 1;
        public float GrassDistanceMultiplier { get; set; } = 1;

        // A note on the values labelled as exponents:
        // I'd like to keep these as sliders in the settings screen,
        // but sliders don't appreciate values which have non-linear steps.
        // So we store them internally as exponents of 2 (since that's what godot limits us to anyway) and use a custom display function for UI
        public int DirectionalShadowSizeExponent { get; set; } = 12;
        public int ShadowAtlasSizeExponent { get; set; } = 12;
        public int ShadowAtlasCubemapSizeExponent { get; set; } = 9;
        // todo: these two probably need to be applied to the individual viewport.
        public AntiAliasingMode AntiAliasingMode { get; set; } = AntiAliasingMode.Fast;
        public Viewport.MSAA Msaa { get; set; } = Viewport.MSAA.Msaa4x;

        public void Apply()
        {
            ProjectSettings.SetSetting("rendering/quality/directional_shadow/size", 1 << DirectionalShadowSizeExponent);
            ProjectSettings.SetSetting("rendering/quality/shadow_atlas/size", 1 << DirectionalShadowSizeExponent);
            ProjectSettings.SetSetting("rendering/quality/shadow_atlas/cubemap_size", 1 << DirectionalShadowSizeExponent);
        }

        public void ApplyToViewport(Viewport viewport)
        {
            if (AntiAliasingMode == AntiAliasingMode.Disabled)
            {
                viewport.Fxaa = false;
                viewport.Msaa = Viewport.MSAA.Disabled;
            }
            else if (AntiAliasingMode == AntiAliasingMode.Fast)
            {
                viewport.Fxaa = true;
                viewport.Msaa = Viewport.MSAA.Disabled;
            }
            else
            {
                viewport.Fxaa = false;
                viewport.Msaa = Msaa;
            }
        }
    }

    public enum AntiAliasingMode
    {
        Disabled,
        Fast,
        High
    }
}