using Godot;
using System;
using System.Collections.Generic;
using Tomlet;

namespace UI.Settings
{
    public partial class GraphicsPreset : SimSettings.GraphicsSettings
    {
        public string Name { get; set; } = "Unnamed preset";
        public string Description { get; set; } = "What type of computer can run this?";

        public static GraphicsPreset Clone(GraphicsPreset graphicsPreset)
        {
            return TomletMain.To<GraphicsPreset>(TomletMain.TomlStringFrom(graphicsPreset));
        }

        public static List<GraphicsPreset> Presets { get; } = new()
        {
            new()
            {
                Name = "Minimal",
                Description = "Absolutely minimal settings, few computers will actually need this",

                UseImpostors = true,
                ImpostorDistance = 0,
                ImpostorShadowsEnabled = false,
                FarVegetationMultiplier = 0,
                NearVegetationMultiplier = 0,
                GrassDistanceMultiplier = 0,
                GrassMultiplier = 0,

                DirectionalShadowSizeExponent = 8,
                ShadowAtlasSizeExponent = 8,
                ShadowAtlasCubemapSizeExponent = 6,

                AntiAliasingMode = SimSettings.AntiAliasingMode.Disabled,
                Msaa = SubViewport.Msaa.Disabled,
            },
            new()
            {
                // target: hd 3000
                Name = "Very Low",
                Description = "For computers with very old, weak hardware - for example integrated graphics 10 years old",

                UseImpostors = true,
                ImpostorDistance = 0,
                ImpostorShadowsEnabled = false,
                FarVegetationMultiplier = 0.1f,
                NearVegetationMultiplier = 0.2f,
                GrassDistanceMultiplier = 0.4f,
                GrassMultiplier = 0.1f,

                DirectionalShadowSizeExponent = 8,
                ShadowAtlasSizeExponent = 8,
                ShadowAtlasCubemapSizeExponent = 6,

                AntiAliasingMode = SimSettings.AntiAliasingMode.Disabled,
                Msaa = SubViewport.Msaa.Disabled,
            },
            new()
            {
                // target: uhd 630
                Name = "Low",
                Description = "For computers with modern integrated graphics or older discrete GPUs",

                UseImpostors = true,
                ImpostorDistance = 0,
                ImpostorShadowsEnabled = false,
                FarVegetationMultiplier = 0.25f,
                NearVegetationMultiplier = 0.4f,
                GrassDistanceMultiplier = 0.35f,
                GrassMultiplier = 0.5f,

                DirectionalShadowSizeExponent = 9,
                ShadowAtlasSizeExponent = 9,
                ShadowAtlasCubemapSizeExponent = 6,

                AntiAliasingMode = SimSettings.AntiAliasingMode.Fast,
                Msaa = SubViewport.Msaa.Disabled,
            },
            new()
            {
                // Target 
                Name = "Medium",
                Description = "For computers with modern budget GPUs, or older mid-range GPUs",

                UseImpostors = true,
                ImpostorDistance = 0,
                ImpostorShadowsEnabled = false,
                FarVegetationMultiplier = 0.1f,
                NearVegetationMultiplier = 0.2f,
                GrassDistanceMultiplier = 0.5f,
                GrassMultiplier = 0.7f,

                DirectionalShadowSizeExponent = 10,
                ShadowAtlasSizeExponent = 10,
                ShadowAtlasCubemapSizeExponent = 7,

                AntiAliasingMode = SimSettings.AntiAliasingMode.Quality,
                Msaa = SubViewport.Msaa.Msaa2X,
            },
            new()
            {
                // Target: rx6600
                Name = "High",
                Description = "For computers with relatively modern mid-range GPUs",

                UseImpostors = true,
                ImpostorDistance = 40,
                ImpostorShadowsEnabled = true,
                FarVegetationMultiplier = 1f,
                NearVegetationMultiplier = 1f,
                GrassDistanceMultiplier = 1f,
                GrassMultiplier = 1f,

                DirectionalShadowSizeExponent = 12,
                ShadowAtlasSizeExponent = 12,
                ShadowAtlasCubemapSizeExponent = 9,

                AntiAliasingMode = SimSettings.AntiAliasingMode.Quality,
                Msaa = SubViewport.Msaa.Msaa4X,
            },
            new()
            {
                Name = "Very High",
                Description = "For computers with modern high-end GPUs, or mid-range GPUs at lower resolutions and refresh rates",

                UseImpostors = true,
                ImpostorDistance = 100,
                ImpostorShadowsEnabled = true,
                FarVegetationMultiplier = 1.2f,
                NearVegetationMultiplier = 1.2f,
                GrassDistanceMultiplier = 1.5f,
                GrassMultiplier = 1.2f,

                DirectionalShadowSizeExponent = 14,
                ShadowAtlasSizeExponent = 14,
                ShadowAtlasCubemapSizeExponent = 12,

                AntiAliasingMode = SimSettings.AntiAliasingMode.Quality,
                Msaa = SubViewport.Msaa.Msaa8X,
            },
        };
    }
}