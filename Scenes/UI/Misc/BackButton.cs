using Godot;
using System;

namespace UI.Misc
{
    public partial class BackButton : MarginContainer
    {
        [Export(PropertyHint.File, hintString: "*.tscn")] public string ScenePath { get; set; }

        private void _on_Button_pressed()
        {
            GetTree().ChangeSceneToFile(ScenePath);
        }
    }
}