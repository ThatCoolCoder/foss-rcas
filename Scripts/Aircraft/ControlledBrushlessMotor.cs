using Godot;
using System;

namespace Aircraft
{
    public partial class ControlledBrushlessMotor : Physics.Motors.BrushlessMotor
    {
        [Export] public string ThrottleActionName { get; set; }
        [Export] public bool Reversible { get; set; }
        [Export] public NodePath ControlHubPath { get; set; }
        private Control.IHub controlHub;

        public override void _Ready()
        {
            controlHub = Utils.GetNodeWithWarnings<Control.IHub>(this, ControlHubPath, "control hub");
            base._Ready();
        }

        public override void _Process(double delta)
        {
            ThrustProportion = controlHub.ChannelValues[ThrottleActionName];
            if (!Reversible) ThrustProportion = ThrustProportion / 2 + 0.5f;

            base._Process(delta);
        }
    }
}