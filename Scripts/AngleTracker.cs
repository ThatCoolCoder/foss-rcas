using Godot;
using System;

public class AngleTracker : Spatial
{
    [Export] public bool Enabled { get; set; } = true;
    [Export] public NodePath TargetPath { get; set; }
    private Spatial target;

    public override void _Ready()
    {
        target = GetNode<Spatial>(TargetPath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Enabled) LookAt(target.GlobalTranslation, Vector3.Up);
    }
}
