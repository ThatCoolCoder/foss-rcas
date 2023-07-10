using Godot;
using System;

namespace UI.Settings
{
    using Components;
    using Settings = SimSettings.Settings;

    public partial class SettingsScreen : Control
    {
        // using statics here makes it so much easier and I don't envisage a scenario where someone would ever want two instances, so that is what is done

        public static Settings NewSettings { get; set; }
        public static event Action OnSettingsChanged;

        public override void _Ready()
        {
            NewSettings = Settings.CloneCurrent();

            OnSettingsChanged(); // update all inputs
        }

        private void _on_ConfirmationDialog_confirmed()
        {
            NewSettings = new Settings();

            _on_Apply_pressed();
        }

        private void _on_Reset_pressed()
        {
            GetNode<Misc.CustomConfirmationDialogue>("CustomConfirmationDialog").AskToConfirm();
        }

        private void _on_Cancel_pressed()
        {
            GetTree().ChangeSceneToFile("res://Scenes/UI/StartScreen.tscn");
        }

        private void _on_Revert_pressed()
        {
            NewSettings = Settings.CloneCurrent();
            if (OnSettingsChanged != null) OnSettingsChanged.Invoke();
        }

        private void _on_Apply_pressed()
        {
            Settings.SetCurrent(NewSettings);
            if (OnSettingsChanged != null) OnSettingsChanged.Invoke();
            Settings.SaveCurrent();
        }

        private void _on_Accept_pressed()
        {
            Settings.SetCurrent(NewSettings);
            if (OnSettingsChanged != null) OnSettingsChanged.Invoke();
            Settings.SaveCurrent();


            GetTree().ChangeSceneToFile("res://Scenes/UI/StartScreen.tscn");
        }

        public override void _ExitTree()
        {
            OnSettingsChanged = null;
        }

        public static void ChangeSettings()
        {
            // Call OnSettingsChanged

            OnSettingsChanged.Invoke();
        }
    }
}