using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Locations
{

    [Tool]
    public partial class ImpostorSprite : Sprite3D
    {
        // Sprite for an impostor tree, randomly chooses an image on Ready

        [Export] public Godot.Collections.Array<Texture2D> Textures { get; set; } = new();

        private int lastTexCount = 0;

        public override void _Ready()
        {
            PickTexture();
            AlphaCut = SimSettings.Settings.Current.Graphics.ImpostorShadowsEnabled ? AlphaCutMode.OpaquePrepass : AlphaCutMode.Disabled;
        }

        private void PickTexture()
        {
            if (Textures.Count > 0) Texture = Utils.RandomItem(Textures);
            else Texture = null;
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint() && Engine.GetFramesDrawn() % 60 == 0)
            {
                if (Textures.Where(x => x != null).Count() != lastTexCount)
                {
                    lastTexCount = Textures.Where(x => x != null).Count();
                    PickTexture();
                }
            }
        }
    }
}