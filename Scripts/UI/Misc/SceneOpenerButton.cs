using Godot;
using System;

namespace UI.Misc
{
    public class SceneOpenerButton : Button
    {
        // Button that opens a scene when clicked, saves work having to 

        [Export(PropertyHint.File, "*.tscn,")] public string ScenePath { get; set; } = "";

        public override void _Ready()
        {
            Connect("pressed", this, "OnClicked");
        }

        public void OnClicked()
        {
            GetTree().ChangeScene(ScenePath);
        }
    }
}