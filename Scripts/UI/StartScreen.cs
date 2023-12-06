using Godot;
using System;
using System.Collections.Generic;
using Tomlet;

namespace UI;

public partial class StartScreen : Control
{
    public override void _Ready()
    {
        SimSettings.Settings.LoadCurrent();
        SimInput.Manager.Instance.LoadInputMap(SimSettings.Settings.Current.InputMap);
    }

    public void _on_AboutButton_pressed()
    {
        OS.ShellOpen("https://github.com/ThatCoolCoder/foss-rcas");
    }
}
