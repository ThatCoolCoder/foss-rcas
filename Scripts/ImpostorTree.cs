using Godot;
using System;

public class ImpostorTree : Spatial
{
    // Very basic impostor/normal model switching intended for trees (currently a drastic last-ditch performance improvement)
    // Expects impostor to be a direct child called "Impostor" and real node to be a direct child called "RealNode"

    [Export] public int UpdateInterval { get; set; } = 10;
    [Export] public float ImpostorDistance { get; set; } = 30;

    private Camera camera;

    private Spatial impostor;
    private Spatial realNode;

    public override void _Ready()
    {
        camera = GetViewport().GetCamera();
        impostor = GetNode<Spatial>("Impostor");
        realNode = GetNode<Spatial>("RealNode");

        UpdateVisibility();
    }

    public override void _Process(float delta)
    {
        if (Engine.GetFramesDrawn() % UpdateInterval == 0)
        {
            UpdateVisibility();
        }
    }

    private void UpdateVisibility()
    {
        if(camera.GlobalTranslation.DistanceSquaredTo(GlobalTranslation) < ImpostorDistance * ImpostorDistance)
        {
            impostor.Visible = false;
            realNode.Visible = true;
        }
        else
        {
            impostor.Visible = true;
            realNode.Visible = false;
        }
    }
}