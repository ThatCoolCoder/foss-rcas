using Godot;
using System;
using Tomlet;

namespace SimSettings
{
    public class GraphicsSettings
    {
        // Impostors as in impostor trees for better performance.
        public bool UseImpostors { get; set; } = true;
        public int ImpostorDistance { get; set; } = 50;
        public bool ShowFps { get; set; } = false;
    }
}