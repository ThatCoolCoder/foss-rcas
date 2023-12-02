using Godot;
using System;
using System.Linq;
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
        foreach (var (operation, index) in Operations.Select((item, index) => (item, index)))
        {
            try
            {
                operation.Execute(this);
            }
            catch (Exception e)
            {
                if (e is not Exceptions.ValueSetterException)
                {
                    e = new Exceptions.ValueSetterException(e.Message, e);
                }
                var valueSetterException = (Exceptions.ValueSetterException)e;
                valueSetterException.OperationName = operation.GetType().Name;
                valueSetterException.OperationNumber = index + 1;

                GD.PrintS($"Error in {operation.GetType().Name} (operation {index + 1}):", e); // todo: get some in-game console
                break;
            }
        }
    }

    public dynamic GetVariable(string name)
    {
        // todo: make error checking
        if (!variables.TryGetValue(name, out var result)) throw new Exception($"Variable \"{name}\" does not exist");
        return result;
    }

    public void SetVariable(string name, dynamic value)
    {
        variables[name] = value;
    }
}