using Godot;
using System;

namespace Aircraft;

public partial class Mirror : Node3D
{
    // Thing that mirrors its parent across the X-axis. Note that it does not mirror the node itself, only its position. (messing with the node scale just broke too many things)

    public override void _Ready()
    {
        var duplicate = (Node3D)GetParent<Node3D>().Duplicate();

        MirrorNode(duplicate);
        duplicate.Name = GetParent<Node3D>().Name + "Mirrored";

        duplicate.GetNode(new NodePath(Name)).Free(); // prevent that from having a mirror node and getting into an infinite loop

        GetParent().GetParent().CallDeferred("add_child", duplicate);
    }

    private void MirrorNode(Node3D node)
    {
        node.Position = node.Position.WithX(-node.Position.X);
        node.Rotation = node.Rotation.WithZ(-node.Rotation.Z);
        foreach (var child in node.GetChildren())
        {
            if (child is Node3D s) MirrorNode(s);
        }
    }


}