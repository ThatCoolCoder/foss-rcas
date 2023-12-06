using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Tomlet;

namespace Aircraft.Control;

public partial class MixerHub : Node3D, IHub
{
    [Export(PropertyHint.File, "*.toml")] public string MixesFile { get; set; }

    protected ChannelMixSet channelMixSet;

    public Dictionary<string, float> ChannelValues { get; set; } = new();
    private HashSet<string> movedActions = new();
    private Dictionary<string, float> initialChannelValues = new();

    public override void _Ready()
    {
        var gdFile = FileAccess.Open(MixesFile, FileAccess.ModeFlags.Read);
        if (gdFile == null)
        {
            Utils.LogError($"Failed opening mixes file ({MixesFile})", this);
            return;
        }

        var content = gdFile.GetAsText();
        gdFile.Close();
        channelMixSet = TomletMain.To<ChannelMixSet>(content);


        initialChannelValues = SimInput.AvailableInputActions.GetActionList()
            .ToDictionary(x => x.Key, x => SimInput.Manager.GetActionValue(x.Key));


        // Get initial values for all channels
        ChannelValues = GetChannelValues(0);
    }

    public override void _Process(double delta)
    {
        ChannelValues = GetChannelValues((float)delta);
    }

    private Dictionary<string, float> GetChannelValues(float delta)
    {
        var newChannelValues = new Dictionary<string, float>();

        foreach (var mix in channelMixSet.Mixes)
        {
            var actionPath = "aircraft/" + mix.InputChannelName;
            var rawInputValue = SimInput.Manager.GetActionValue(actionPath);

            if (rawInputValue != initialChannelValues[actionPath]) movedActions.Add(actionPath);

            // If not moved & we have a custom default, use that instead of normal value
            var usedInputValue = !movedActions.Contains(actionPath) && channelMixSet.CustomDefaultValues.TryGetValue(mix.InputChannelName, out var customDefault)
                ? customDefault
                : rawInputValue;

            // If there was already stuff from the previous mix in the list, apply on top of that
            if (newChannelValues.TryGetValue(mix.OutputChannelName, out var valueFromPreviousMix))
            {
                newChannelValues[mix.OutputChannelName] = mix.Apply(usedInputValue, valueFromPreviousMix, delta);
            }
            // Otherwise just apply as a fresh value
            else
            {
                newChannelValues[mix.OutputChannelName] = mix.Apply(usedInputValue, 0, delta);
            }
        }

        return newChannelValues.ToDictionary(kvp => kvp.Key, kvp => Mathf.Clamp(kvp.Value, -1, 1));
    }
}