using Godot;
using System;

public class Mirror : Spatial
{
    // Thing that mirrors its parent across the X-axis.

    public override void _Ready()
    {
        var duplicate = (Spatial)GetParent<Spatial>().Duplicate();

        duplicate.Translation = duplicate.Translation.WithX(-duplicate.Translation.x);
        duplicate.Scale = duplicate.Scale.WithX(-duplicate.Scale.x);
        duplicate.Rotation = duplicate.Rotation.WithX(-duplicate.Rotation.z);
        duplicate.Name = GetParent<Spatial>().Name + "Mirrored";

        duplicate.GetNode(new NodePath(Name)).Free(); // prevent that from having a mirror node and getting into an infinite loop

        GetParent().GetParent().CallDeferred("add_child", duplicate);
    }


}