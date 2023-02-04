using Godot;
using System;


namespace UI.Settings
{
    using Components;

    public class MiscTab : VBoxContainer
    {

        public override void _Ready()
        {
            NumericInput.Scene.Instance<NumericInput>().Config(
                this,
                "Physics FPS",
                s => s.PhysicsFps,
                (s, v) => s.PhysicsFps = (int)v,
                30, 1000, step: 1,
                toolTip: "Higher values give a more accurate simulation but may cause poor performance on lower-end hardware");

            FileInput.Scene.Instance<FileInput>().Config(
                this,
                "Add on directory",
                s => s.AddOnRepositoryPath,
                (s, v) => s.AddOnRepositoryPath = v,
                _mode: FileDialog.ModeEnum.OpenDir,
                toolTip: "(doesn't do anything yet)");

            BooleanInput.Scene.Instance<BooleanInput>().Config(
                this,
                "FPS counter enabled",
                s => s.ShowFps,
                (s, v) => s.ShowFps = v,
                toolTip: "Whether to show the frames per second on screen");
        }

    }
}