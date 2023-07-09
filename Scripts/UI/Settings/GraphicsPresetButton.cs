using Godot;
using System;

namespace UI.Settings
{
    public partial class GraphicsPresetButton : Button
    {
        public GraphicsPreset GraphicsPreset { get; set; }

        public event Action<GraphicsPreset> OnClicked;

        public override void _Ready()
        {
            if (GraphicsPreset == null) Utils.LogError("GraphicsPreset is null", this);
            Text = GraphicsPreset.Name;
            TooltipText = GraphicsPreset.Description;
        }

        private void _on_GraphicsPresetButton_pressed()
        {
            if (OnClicked != null) OnClicked(GraphicsPreset);
        }
    }
}