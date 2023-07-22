using Godot;
using System;
using System.Linq;

namespace UI;

public partial class AboutScreen : Control
{
    [Export] private Label licenseLabel;
    [Export] private RichTextLabel assetsCreditsLabel;

    public override void _Ready()
    {
        using var licenseFile = FileAccess.Open("res://LICENSE", FileAccess.ModeFlags.Read);
        licenseLabel.Text = licenseFile.GetAsText();
        using var assetsCreditsFile = FileAccess.Open("res://CREDITS_ART.txt", FileAccess.ModeFlags.Read);
        assetsCreditsLabel.Text = String.Join("\n", assetsCreditsFile.GetAsText().Split("\n").Where(x => x != "").Skip(1));
    }

    private void LabelMetaClicked(string meta)
    {
        OS.ShellOpen(meta);
    }
}