using Godot;
using System;

public class Heading : CenterContainer
{
    // Basic page heading 

    [Export]
    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
            if (label != null) label.Text = _text;
        }
    }
    private string _text = "Heading";

    private Label label;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        label.Text = _text;
    }

}
