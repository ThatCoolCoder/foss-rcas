using Godot;
using System;

namespace Physics.Forcers
{
	public class ControlledMotor : Motor
	{
		// Motor controlled by the keyboard

		[Export] public string ThrottleActionName { get; set; }
		[Export] public bool Reversible { get; set; }

		public override void _Process(float delta)
		{
			ThrustProportion = SimInputManager.GetAxisValue("throttle");
			if (ThrustProportion == 0) ThrustProportion = -1; // hacky thing to make it not start at init
			if (! Reversible) ThrustProportion = ThrustProportion / 2 + 0.5f;
			base._Process(delta);
		}
	}
}
