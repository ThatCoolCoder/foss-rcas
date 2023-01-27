using Godot;
using System;

public class ControlSurface : Spatial
{
    // Object that rotates around its X-axis to 

    [Export] public float MaxDeflectionDegrees { get; set; } = 30;
    [Export] public bool Reversed { get; set; }
    [Export] public string ActionName { get; set; } = "";

    public override void _Process(float delta)
    {
        var rawValue = SimInput.Manager.GetAxisValue(ActionName);
        rawValue *= Mathf.Deg2Rad(MaxDeflectionDegrees);
        if (Reversed) rawValue *= -1;
        Rotation = Rotation.WithX(rawValue);
    }

}
