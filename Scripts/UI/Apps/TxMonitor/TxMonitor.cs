using Godot;
using System;

namespace UI.Apps.TxMonitor;

[Tool]
public partial class TxMonitor : Misc.UserManipulate
{
    // Little transmitter that shows the stick positions and such

    [Export] private SubViewportContainer container;
    [Export] private SubViewport viewport;
    [Export] private Node2D viewportRootNode;

    public override void _Process(double delta)
    {
        container.CustomMinimumSize = new Vector2(container.Size.Y, 0);
        viewportRootNode.Scale = Mathf.Min(viewport.Size.X, viewport.Size.Y) / 256f * Vector2.One;

        base._Process(delta);
    }
}