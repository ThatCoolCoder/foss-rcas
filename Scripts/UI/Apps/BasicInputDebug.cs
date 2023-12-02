using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Apps;

public partial class BasicInputDebug : Misc.UserManipulate
{
    // Basic implementation just visually displaying values of the aircraft channels.

    [Export] public Godot.Collections.Array<string> ChannelNames { get; set; }
    private RichTextLabel label;

    public override void _Ready()
    {
        label = GetNode<RichTextLabel>("Panel/MarginContainer/VBoxContainer/RichTextLabel");
        label.BbcodeEnabled = true;

        base._Ready();
    }

    public override void _Process(double delta)
    {
        var text = String.Join("\n", ChannelNames.Select(action =>
        {
            var rawValue = SimInput.Manager.GetActionValue("aircraft/" + action);
            var displayedValue = Utils.RoundToPlaces(rawValue * 100, 0);
            var name = action.Split("/").Last();
            return $"{name}: {displayedValue:+0;-#}%";
        }));

        label.Text = text;

        base._Process(delta);
    }
}