using Godot;
using System;
using System.Collections.Generic;

namespace Aircraft.ValueSetters;

public partial class ValueSetter : Node3D
{
    [Export] public bool Enabled { get; set; } = true;
    [Export] public UpdateModeEnum UpdateMode { get; set; } = UpdateModeEnum.Process;
    [Export] public float UpdateInterval { get; set; } = 1;
    [Export] public Godot.Collections.Array<Operations.AbstractValueSetterOperation> Operations { get; set; }

    private Dictionary<string, dynamic> variables = new();

    public enum UpdateModeEnum
    {
        ResetOnly,
        Process,
        PhysicsProcess
    }

    public override void _Ready()
    {
        if (Enabled) CallDeferred("Update");
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
        variables = new();
        foreach (var operation in Operations) operation.Execute(this);
    }

    public dynamic GetVariable(string name)
    {
        // todo: make error checking
        return variables[name];
    }

    public void SetVariable(string name, dynamic value)
    {
        variables[name] = value;
    }
}