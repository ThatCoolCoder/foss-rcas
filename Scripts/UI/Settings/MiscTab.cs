using Godot;
using System;


namespace UI.Settings
{
    using Components;

    public class MiscTab : Control
    {

        public override void _Ready()
        {
            var holder = GetNode<Control>("MaxSizeContainer/VBoxContainer");

            NumericInput.Scene.Instance<NumericInput>().Config(
                holder,
                "Physics FPS",
                s => s.Misc.PhysicsFps,
                (s, v) => s.Misc.PhysicsFps = (int)v,
                30, 1000, step: 1,
                toolTip: "Higher values give a more accurate simulation but may cause poor performance on lower-end hardware");

            FileInput.Scene.Instance<FileInput>().Config(
                holder,
                "Add on directory",
                s => s.Misc.AddOnRepositoryPath,
                (s, v) => s.Misc.AddOnRepositoryPath = v,
                _mode: FileDialog.ModeEnum.OpenDir,
                toolTip: "(doesn't do anything yet)");
        }

    }
}