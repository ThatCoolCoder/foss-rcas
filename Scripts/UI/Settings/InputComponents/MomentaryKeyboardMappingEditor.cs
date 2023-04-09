using Godot;
using System;

namespace UI.Settings.InputComponents
{
    using Components;

    public class MomentaryKeyboardMappingEditor : BaseControlMappingEditor
    {
        // It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
        // The performance should still be fine and this makes it SO much easier to code.

        private SimInput.MomentaryKeyboardControlMapping controlMapping;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/MomentaryKeyboardMappingEditor.tscn");

        public MomentaryKeyboardMappingEditor Config(Node parent, SimInput.MomentaryKeyboardControlMapping _controlMapping)
        {
            controlMapping = _controlMapping;
            if (parent != null) parent.AddChild(this);

            return this;
        }

        public override void _Ready()
        {
            var holder = GetMainItemHolder();

            holder.GetNode<KeyInput>("KeyInput").Config(null, "Key",
                s => controlMapping.KeyScancode,
                (s, v) => controlMapping.KeyScancode = v).OnSettingsChanged();
        }
    }
}
