using System;
using Godot;

namespace Physics.Fluids
{
    [Tool]
    public partial class Water : MeshInstance3D, ISpatialFluid
    {
        // Water with pretty mediocre ripples 
        // We have copies of the Textures as Images so we can import them from GPU to CPU.

        // IMPORTANT: when modifying the math, please modify water.gdshader to reflect the changes,
        // or physics and graphics will desync!

        // todo: make better "physics" and tie it to wind
        // todo: make this a tool and configurable in editor without messing with the mesh's inner properties

        [Export] public float BaseDensity { get; set; } = 1000.0f;
        [Export] public Vector3 Flow { get; set; } = Vector3.Zero;


        #region ShaderParameters
        private ShaderMaterial material;

        [Export] public float VertexScale;
        [Export] public float HeightScale;
        [Export] public Texture2D Noise;
        [Export] public int NoiseSize;
        private Image noiseImage;
        public float Time = 0;

        [Export] public int WaveMapSize;
        [Export] public Texture2D WaveMap1;
        private Image waveMap1Image;
        [Export] public Texture2D WaveMap2;
        private Image waveMap2Image;
        [Export] public Texture2D WaveMap3;
        private Image waveMap3Image;

        [Export] public Vector2 WaveAngle1 = new Vector2(0, 1);
        [Export] public Vector2 WaveAngle2 = new Vector2(0.5f, 0.866f);
        [Export] public Vector2 WaveAngle3 = new Vector2(-0.5f, 0.866f);

        [Export] public float WaveSpeed;
        [Export] public float WaveHeightScale;

        [Export] public float DistanceFadeStart = 100;
        [Export] public float DistanceFadeEnd = 200;
        #endregion ShaderParameters

        private float normalSampleDistance = 0.1f;
        private readonly Vector3 degrees120 = new Vector3(1, 0, 0).Rotated(Vector3.Up, 120);
        private readonly Vector3 degrees240 = new Vector3(1, 0, 0).Rotated(Vector3.Up, 240);

        public override void _Ready()
        {
            UpdateShaderParams();
            if (!Engine.IsEditorHint()) base._Ready();
            Time = 0;
        }

        private void UpdateShaderParams()
        {
            material = GetSurfaceOverrideMaterial(0) as ShaderMaterial;

            material.SetShaderParameter("scale", VertexScale);
            material.SetShaderParameter("height_scale", HeightScale);
            material.SetShaderParameter("noise_size", NoiseSize);
            material.SetShaderParameter("noise", Noise);

            material.SetShaderParameter("wave_map_size", WaveMapSize);
            material.SetShaderParameter("wave_height_1", WaveMap1);
            material.SetShaderParameter("wave_height_2", WaveMap2);
            material.SetShaderParameter("wave_height_3", WaveMap3);

            material.SetShaderParameter("wave_angle_1", WaveAngle1);
            material.SetShaderParameter("wave_angle_2", WaveAngle2);
            material.SetShaderParameter("wave_angle_3", WaveAngle3);

            material.SetShaderParameter("wave_speed", WaveSpeed);
            material.SetShaderParameter("wave_height_scale", WaveHeightScale);

            material.SetShaderParameter("distance_fade_start", DistanceFadeStart);
            material.SetShaderParameter("distance_fade_end", DistanceFadeEnd);
        }

        private void TryGetNoiseImagesFromGpu()
        {
            // Attempt to get the noise images from the gpu.
            noiseImage = Noise.GetImage();
            waveMap1Image = WaveMap1.GetImage();
            waveMap2Image = WaveMap2.GetImage();
            waveMap3Image = WaveMap3.GetImage();
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint() && Engine.GetFramesDrawn() % 10 == 0) UpdateShaderParams();

            Time += (float)delta;
            if (material != null) material.SetShaderParameter("global_time", Time);
        }

        public float HeightAtPoint(Vector3 point)
        {
            if (noiseImage == null || waveMap1Image == null || waveMap2Image == null || waveMap3Image == null)
            {
                GD.Print("No data yet!");
                TryGetNoiseImagesFromGpu();
                return GlobalPosition.Y;
            }

            float ReadPixelValue(Image image, Vector2 uv)
            {
                var x = Mathf.PosMod(uv.X * image.GetWidth(), image.GetWidth());
                var y = Mathf.PosMod(uv.Y * image.GetHeight(), image.GetHeight());
                var pixel = image.GetPixel((int)x, (int)y);
                return pixel.R;
            }

            Vector2 TexturePosFromWorld(Vector2 pos)
            {
                pos /= VertexScale * 2;
                pos.X += 0.5f;
                pos.Y += 0.5f;
                return pos;
            }

            float WaveHeightOffset(Vector2 pos, Image heightMap, Vector2 waveDirection, float time)
            {
                var movement = waveDirection.Normalized() * time * WaveSpeed;
                pos += movement;
                var normalizedPos = pos / WaveMapSize;

                var height = ReadPixelValue(heightMap, normalizedPos);
                height -= 0.5f;
                height *= WaveHeightScale / HeightScale;
                return height;
            }

            float HeightAtPos(Vector2 pos, float time)
            {
                var normalizedPos = TexturePosFromWorld(pos);
                float height = ReadPixelValue(noiseImage, normalizedPos) - 0.5f;
                height += WaveHeightOffset(pos, waveMap1Image, WaveAngle1, time);
                height += WaveHeightOffset(pos, waveMap2Image, WaveAngle2, time);
                height += WaveHeightOffset(pos, waveMap3Image, WaveAngle3, time);

                return height * HeightScale;
            }

            return GlobalPosition.Y + HeightAtPos(new Vector2(point.X, point.Z), Time);
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.Y <= BoundaryAtPoint(point).Y;
        }

        public Vector3 BoundaryAtPoint(Vector3 point)
        {
            return point.WithY(HeightAtPoint(point));
        }

        public float DensityAtPoint(Vector3 point)
        {
            return BaseDensity;
        }

        public Vector3 VelocityAtPoint(Vector3 point)
        {
            return Flow;
        }

        public Vector3 NormalAtPoint(Vector3 point)
        {
            return Vector3.Up;
        }

        public bool HasBoundaries { get; set; } = true;
    }
}
