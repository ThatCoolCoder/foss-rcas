using Godot;
using System;

namespace UI.Apps.TxMonitor;

[Tool]
public partial class StickMonitor : Node2D
{
    // One of the sticks of the TxMonitor
    [Export] public string VerticalChannelName { get; set; } = "throttle";
    [Export] public string HorizontalChannelName { get; set; } = "rudder";
    [Export] public float HalfMovementDistance { get; set; } = 20;
    [Export] public float EditorOverridePosition { get; set; } = 0;

    private Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public override void _Process(double delta)
    {
        var xChannelValue = Engine.IsEditorHint() ? EditorOverridePosition : SimInput.Manager.GetActionValue("aircraft/" + HorizontalChannelName);
        var yChannelValue = Engine.IsEditorHint() ? EditorOverridePosition : SimInput.Manager.GetActionValue("aircraft/" + VerticalChannelName);

        xChannelValue = Mathf.Clamp(xChannelValue, -1, 1);
        yChannelValue = Mathf.Clamp(yChannelValue, -1, 1);

        sprite.Position = new Vector2(xChannelValue, -yChannelValue) * HalfMovementDistance;
    }
}