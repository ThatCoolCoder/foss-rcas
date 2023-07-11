using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Locations;

[Tool]
public partial class ImpostorMultiSprite : ImpostorSprite
{
    // Sprite for an impostor tree, randomly chooses an image on Ready

    [Export] public Godot.Collections.Array<Texture2D> Textures { get; set; } = new();
    [Export] public Godot.Collections.Array<Texture2D> NormalTextures { get; set; } = new();

    private int lastTexCount = 0;

    public override void _Ready()
    {
        PickTexture();
        base._Ready();
    }

    private void PickTexture()
    {
        if (Textures.Count > 0)
        {
            var index = (int)(GD.Randi() % Textures.Count);
            Texture = Textures[index];

            if (index < NormalTextures.Count) NormalTexture = NormalTextures[index];
            else NormalTexture = null;
        }
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