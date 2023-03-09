using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

using Tomlet;

namespace Aircraft
{
    public class ControlHub : Spatial
    {
        [Export(PropertyHint.File, "*.toml")] public string MixesFile { get; set; }

        private ChannelMixSet channelMixSet;
        private Dictionary<string, ChannelMix> inputChannelNameToMix = new();

        public Dictionary<string, float> ChannelValues { get; set; } = new();

        public override void _Ready()
        {
            var gdFile = new File();
            gdFile.Open(MixesFile, File.ModeFlags.Read);
            var content = gdFile.GetAsText();
            gdFile.Close();
            channelMixSet = TomletMain.To<ChannelMixSet>(content);

            inputChannelNameToMix = channelMixSet.Mixes.ToDictionary(m => m.InputChannelName, m => m);
        }

        public override void _Process(float delta)
        {
            foreach (var mix in channelMixSet.Mixes)
            {
                var rawValue = SimInput.Manager.GetAxisValue(mix.InputChannelName);
                ChannelValues[mix.OutputChannelName] = mix.Apply(rawValue);
            }
        }
    }
}