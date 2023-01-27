using Godot;
using System;

public class World : Spatial
{
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("reset")) GetTree().ReloadCurrentScene();
	}
}
