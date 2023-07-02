using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Apps
{
    public abstract class AbstractBasicNodeDebug<T> : Control where T : Node
    {
        // Simple base debugger for items like motors or batteries

        [Export] public int RescanFrequency = 240;
        protected abstract string GroupName { get; set; } // name of group containing items
        private Label label;
        private SpinBox nodeIndexSelector;
        private List<NodePath> foundNodes = new();
        private NodePath currentNodePath;

        public override void _Ready()
        {
            label = GetNode<Label>("MarginContainer/VBoxContainer/Output");
            nodeIndexSelector = GetNode<SpinBox>("MarginContainer/VBoxContainer/HBoxContainer/SpinBox");

            ScanForNode();
        }

        public override void _Process(float delta)
        {
            if (Engine.GetFramesDrawn() % RescanFrequency == 0) ScanForNode();

            var text = "Not found";
            if (currentNodePath != null)
            {
                var node = GetNode<T>(currentNodePath);
                if (node != null) text = GenerateText(node);
            }
            label.Text = text;
        }

        protected abstract string GenerateText(T node);

        private void ScanForNode()
        {
            foundNodes = GetTree().GetNodesInGroup(GroupName).ToList<Node>().Select(x => x.GetPath()).ToList();
            if (foundNodes.Count > 0)
            {
                nodeIndexSelector.MinValue = 0;
                nodeIndexSelector.MaxValue = foundNodes.Count - 1;
                nodeIndexSelector.Editable = true;
            }
            else
            {
                nodeIndexSelector.Editable = false;
            }
        }

        private void _on_SpinBox_changed()
        {
            currentNodePath = foundNodes[(int)nodeIndexSelector.Value];
        }
    }
}