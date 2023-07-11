using Godot;
using System;

namespace Locations
{
    [Tool]
    public partial class ImpostorSprite : MeshInstance3D
    {
        // todo: add billboard functionality

        [Export]
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                if (Mesh is QuadMesh qm && qm.Material is StandardMaterial3D sm)
                {
                    sm.AlbedoTexture = value;
                }
            }
        }
        private Texture2D texture;

        [Export]
        public Texture2D NormalTexture
        {
            get { return normalTexture; }
            set
            {
                normalTexture = value;
                if (Mesh is QuadMesh qm && qm.Material is StandardMaterial3D sm)
                {
                    sm.NormalTexture = value;
                    sm.NormalEnabled = NormalTexture != null;
                }
            }
        }
        private Texture2D normalTexture;

        [Export]
        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                if (Mesh is QuadMesh qm) qm.Size = value;
            }
        }
        private Vector2 size;

        public override void _Ready()
        {
            var quadMesh = new QuadMesh()
            {
                Size = Size
            };

            quadMesh.Material = new StandardMaterial3D()
            {
                AlbedoTexture = Texture,
                NormalEnabled = NormalTexture != null,
                NormalTexture = NormalTexture,
                CullMode = BaseMaterial3D.CullModeEnum.Disabled,
                Transparency = BaseMaterial3D.TransparencyEnum.AlphaDepthPrePass
            };
            Mesh = quadMesh;
        }
    }
}