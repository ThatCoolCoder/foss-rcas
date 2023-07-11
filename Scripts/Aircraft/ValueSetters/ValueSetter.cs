using Godot;
using System;
using System.Linq;

namespace Aircraft.ValueSetters;

public partial class ValueSetter : Node3D
{
    // [ExportGroup("Update settings")]
    [Export] public bool Enabled { get; set; } = true;
    [Export] public UpdateModeEnum UpdateMode { get; set; } = UpdateModeEnum.Process;
    [Export] public float UpdateInterval { get; set; } = 1;

    // [ExportGroup("Source")]
    [Export] public Sources.AbstractValueSource Source { get; set; }
    // [ExportGroup("Transformations")]
    [Export] public Godot.Collections.Array<Transformations.AbstractValueTransformation> Transformations { get; set; }
    // [ExportGroup("Output")]
    [Export] public Sources.AbstractValueSetterOutput Output { get; set; }

    public enum UpdateModeEnum
    {
        ResetOnly,
        Process,
        PhysicsProcess
    }

    public override void _Ready()
    {
        CallDeferred("Update");
    }

    public override void _Process(double delta)
    {
        if (Enabled && UpdateMode == UpdateModeEnum.Process && Engine.GetFramesDrawn() % UpdateInterval == 0) Update();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Enabled && UpdateMode == UpdateModeEnum.PhysicsProcess && Engine.GetPhysicsFrames() % UpdateInterval == 0) Update();
    }

    private void Update()
    {
        var value = Source.GetValue();
        foreach (var transformation in Transformations) value = transformation.Apply(value);
        Output.Apply(value);
    }
}