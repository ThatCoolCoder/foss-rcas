using Godot;
using System;

namespace UI.Apps.TxMonitor;

[Tool]
public partial class AuxMonitor : ProgressBar
{
    // Aux channel, in a progress bar
    [Export] public string ChannelName { get; set; } = "aux1";
    [Export] public float EditorOverridePosition { get; set; } = 0;
    [Export] private Label label;

    public override void _Ready()
    {
        label.Text = ChannelName;
    }

    public override void _Process(double delta)
    {
        var channelValue = Engine.IsEditorHint() ? EditorOverridePosition : SimInput.Manager.GetActionValue("aircraft/" + ChannelName);

        Value = channelValue;
    }
}