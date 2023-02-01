using Godot;
using System;

namespace UI.Settings
{
    using Components;
    using Settings = SimSettings.Settings;
    
    public class SettingsScreen : Control
    {
        // using statics here makes it so much easier and I don't envisage a scenario where someone would ever want two instances, so that is what is done

        public static Settings NewSettings { get; set; }
        public static event Action OnSettingsChanged;

        public override void _Ready()
        {
            NewSettings = Settings.CloneCurrent();

            AddChild(new )
        }

        public void _on_Revert_pressed()
        {
            NewSettings = Settings.CloneCurrent();
            if (OnSettingsChanged != null) OnSettingsChanged.Invoke();
        }

        public void _on_Apply_pressed()
        {
            Settings.SetCurrent(NewSettings);
            if (OnSettingsChanged != null) OnSettingsChanged.Invoke();
        }

        public void _on_Accept_pressed()
        {
            Settings.SetCurrent(NewSettings);
            if (OnSettingsChanged != null) OnSettingsChanged.Invoke();

            GetTree().ChangeScene("res://Scenes/UI/StartScreen.tscn");
        }

        public override void _ExitTree()
        {
            OnSettingsChanged = null;
        }
    }
}