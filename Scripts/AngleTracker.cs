using Godot;
using System;

public class AngleTracker : Spatial
{
    [Export] public bool Enabled { get; set; } = true;
    [Export] public NodePath TargetPath { get; set; }
    public Spatial Target { get; set; }

    public override void _Ready()
    {
        if (TargetPath != null) Target = GetNode<Spatial>(TargetPath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Enabled && Target != null) LookAt(Target.GlobalTranslation, Vector3.Up);
    }
}
