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
        // a computer with a mid range discrete gpu - EG rtx 3050 rx6600 can achieve the target fps
        // todo: put this info in the location-creation documentation when that is made
        public float FarVegetationMultiplier { get; set; } = 1;
        public float NearVegetationMultiplier { get; set; } = 1;
        public float GrassMultiplier { get; set; } = 1;
        public float GrassDistanceMultiplier { get; set; } = 1;
    }
}