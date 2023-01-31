using Godot;
using System;
using Tomlet;

namespace UI
{
    public class StartScreen : Control
    {
        public override void _Ready()
        {
            SimSettings.Settings.LoadCurrentSettings();
        }

        public void _on_PlayButton_pressed()
        {
            GetTree().ChangeScene("res://Scenes/UI/ConfigScreen.tscn");
        }

        public void _on_AboutButton_pressed()
        {
            OS.ShellOpen("https://github.com/ThatCoolCoder/foss-rcas");
        }
    }
}