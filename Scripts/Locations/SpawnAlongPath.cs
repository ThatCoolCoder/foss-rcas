using Godot;
using System;

public class SpawnAlongPath : Path
{
    [Export] public PackedScene Scene { get; set; }
    [Export] public int NumInstances { get; set; } = 3;
    [Export] public float InstanceScale { get; set; } = 1;
    [Export] public float ScaleVariation { get; set; } = 0.5f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        for (int i = 0; i < NumInstances; i++)
        {
            var position = Curve.InterpolateBaked(GD.Randf() * Curve.GetBakedLength(), cubic: true);
            position.x += (float)GD.RandRange(-2, 2);
            position.z += (float)GD.RandRange(-2, 2);
            var child = Scene.Instance<Spatial>();
            AddChild(child);
            child.GlobalTranslation = position;
            child.Rotation = child.Rotation.WithY(GD.Randf() * Mathf.Tau);
            child.Scale = Vector3.One * (GD.Randf() * ScaleVariation + 1) * InstanceScale;
        }
    }
}
