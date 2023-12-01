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

    public override void _Ready()
    {
        var gdFile = FileAccess.Open(MixesFile, FileAccess.ModeFlags.Read);
        if (gdFile == null) Utils.LogError($"Failed opening mixes file ({MixesFile})", this);
        else
        {
            var content = gdFile.GetAsText();
            gdFile.Close();
            channelMixSet = TomletMain.To<ChannelMixSet>(content);

            // return;
            // Get initial values for all channels
            ChannelValues = GetChannelValues(0);

            // Override custom defaults
            foreach (var kvp in channelMixSet.CustomDefaultValues)
            {
                ChannelValues[kvp.Key] = kvp.Value;
            }
        }
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
            newChannelValues.TryGetValue(mix.OutputChannelName, out var previousValue);

            var actionPath = "aircraft/" + mix.InputChannelName;
            var rawValue = SimInput.Manager.GetActionValue(actionPath);

            if (ChannelValues.TryGetValue(mix.OutputChannelName, out var existingValue) && !SimInput.Manager.HasActionBeenUsed(actionPath))
            {
                // Avoid putting in this case as it would overwrite the custom defaults with the standard defaults
                newChannelValues[mix.OutputChannelName] = existingValue;
            }
            else
            {
                newChannelValues[mix.OutputChannelName] = mix.Apply(rawValue, previousValue, delta);
            }
        }

        return newChannelValues.ToDictionary(kvp => kvp.Key, kvp => Mathf.Clamp(kvp.Value, -1, 1));
    }
}