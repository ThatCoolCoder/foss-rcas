using Godot;
using System;

namespace Aircraft
{
    public class Mirror : Spatial
    {
        // Thing that mirrors its parent across the X-axis. Note that it does not mirror the node itself, only its position. (messing with the node scale just broke too many things)

        public override void _Ready()
        {
            var duplicate = (Spatial)GetParent<Spatial>().Duplicate();

            MirrorNode(duplicate);
            duplicate.Name = GetParent<Spatial>().Name + "Mirrored";

            duplicate.GetNode(new NodePath(Name)).Free(); // prevent that from having a mirror node and getting into an infinite loop

            GetParent().GetParent().CallDeferred("add_child", duplicate);
        }

        private void MirrorNode(Spatial node)
        {
            node.Translation = node.Translation.WithX(-node.Translation.x);
            node.Rotation = node.Rotation.WithZ(-node.Rotation.z);
            foreach (var child in node.GetChildren())
            {
                if (child is Spatial s) MirrorNode(s);
            }
        }


    }
}