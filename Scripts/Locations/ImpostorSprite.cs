using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Locations
{

    [Tool]
    public class ImpostorSprite : Sprite3D
    {
        // Sprite for an impostor tree, randomly chooses an image on Ready

        [Export] public List<Texture> Textures { get; set; } = new();

        private int lastTexCount = 0;

        public override void _Ready()
        {
            PickTexture();
        }

        private void PickTexture()
        {
            if (Textures.Count > 0) Texture = Utils.RandomItem(Textures);
            else Texture = null;
        }

        public override void _Process(float delta)
        {
            if (Engine.EditorHint && Engine.GetFramesDrawn() % 60 == 0)
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