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
        if (gdFile == null) Utils.LogError("Failed opening mixes file ({MixesFile})", this);
        else
        {
            var content = gdFile.GetAsText();
            gdFile.Close();
            channelMixSet = TomletMain.To<ChannelMixSet>(content);
        }
    }

    public override void _Process(double delta)
    {
        var newChannelValues = new Dictionary<string, float>();

        foreach (var mix in channelMixSet.Mixes)
        {
            float previousValue = 0;
            newChannelValues.TryGetValue(mix.OutputChannelName, out previousValue);

            var rawValue = SimInput.Manager.GetActionValue("aircraft/" + mix.InputChannelName);
            newChannelValues[mix.OutputChannelName] = mix.Apply(rawValue, previousValue, (float)delta);
        }

        ChannelValues = newChannelValues.ToDictionary(kvp => kvp.Key, kvp => Mathf.Clamp(kvp.Value, -1, 1));
    }
}