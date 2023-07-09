using Godot;
using System;
using Tomlet;

namespace SimSettings
{
    public partial class GraphicsSettings
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
        public SubViewport.Msaa Msaa { get; set; } = SubViewport.Msaa.Msaa4X;

        public void Apply()
        {
            ProjectSettings.SetSetting("rendering/quality/directional_shadow/size", 1 << DirectionalShadowSizeExponent);
            ProjectSettings.SetSetting("rendering/quality/shadow_atlas/size", 1 << DirectionalShadowSizeExponent);
            ProjectSettings.SetSetting("rendering/quality/shadow_atlas/cubemap_size", 1 << DirectionalShadowSizeExponent);
        }

        public void ApplyToViewport(SubViewport viewport)
        {
            // convtodo: this doesn't appreciate existing
            return;
            if (AntiAliasingMode == AntiAliasingMode.Disabled)
            {
                viewport.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled;
                viewport.Msaa3D = SubViewport.Msaa.Disabled;
            }
            else if (AntiAliasingMode == AntiAliasingMode.Fast)
            {
                viewport.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa;
                viewport.Msaa3D = SubViewport.Msaa.Disabled;
            }
            else
            {
                viewport.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled;
                viewport.Msaa3D = Msaa;
            }
        }
    }

    public enum AntiAliasingMode
    {
        Disabled,
        Fast,
        Quality
    }
}