using Godot;
using System;

public class ImpostorTree : Spatial
{
    // Very basic impostor/normal model switching intended for trees (currently a drastic last-ditch performance improvement)
    // Expects impostor to be a direct child called "Impostor" and real node to be a direct child called "RealNode"

    [Export] public int UpdateInterval { get; set; } = 60;
    [Export] public bool UseGraphicsSettings { get; set; } = true; // Whether to use the graphics settings to override custom settings
    [Export] public bool Enabled { get; set; } = true;
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
        var showImpostor = false;
        var trulyEnabled = UseGraphicsSettings ? SimSettings.Settings.Current.Graphics.UseImpostors : Enabled;
        if (trulyEnabled)
        {
            var trueDistance = UseGraphicsSettings ? SimSettings.Settings.Current.Graphics.ImpostorDistance : ImpostorDistance;
            showImpostor = camera.GlobalTranslation.DistanceSquaredTo(GlobalTranslation) > trueDistance * trueDistance;
        }

        if (showImpostor)
        {
            impostor.Visible = true;
            realNode.Visible = false;
        }
        else
        {
            impostor.Visible = false;
            realNode.Visible = true;
        }
    }
}