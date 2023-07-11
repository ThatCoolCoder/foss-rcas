using Godot;
using System;


namespace UI.Settings;

using Components;

public partial class MiscTab : Control
{
    [Export] public Control Holder { get; set; }

    public override void _Ready()
    {

        NumericInput.Scene.Instantiate<NumericInput>().Config(
            Holder,
            "Physics FPS",
            s => s.Misc.PhysicsFps,
            (s, v) => s.Misc.PhysicsFps = (int)v,
            30, 1000, step: 1,
            toolTip: "Higher values give a more accurate simulation but may cause poor performance on lower-end hardware");

        FileInput.Scene.Instantiate<FileInput>().Config(
            Holder,
            "Add on directory",
            s => s.Misc.AddonRepositoryPath,
            (s, v) => s.Misc.AddonRepositoryPath = v,
            _mode: FileDialog.FileModeEnum.OpenDir,
            toolTip: "(doesn't do anything yet)");
    }

}