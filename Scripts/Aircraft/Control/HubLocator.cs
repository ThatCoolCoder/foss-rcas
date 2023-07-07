using Godot;
using System;

namespace Aircraft.Control
{
    public class HubLocator : Spatial
    {
        // Node that gets a control hub from a NodePath, then assigns it to the parent of this node. 

        [Export] public NodePath ControlHubPath { get; set; }

        public override void _Ready()
        {
            var parent = GetParent();
            if (parent is IControllable controllable)
            {
                var foundNode = GetNode(ControlHubPath);
                if (foundNode is IHub controlHub)
                {
                    controllable.ControlHub = controlHub;
                }
                else
                {
                    Utils.LogError($"{ControlHubPath} was not a ControlHub", this);
                }
            }
            else
            {
                Utils.LogError("ControlHubLocator needs to be the child of an IControllable", this);
            }
        }
    }
}