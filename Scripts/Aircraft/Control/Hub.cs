using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

using Tomlet;

namespace Aircraft.Control
{
    public class Hub : Spatial
    {
        [Export(PropertyHint.File, "*.toml")] public string MixesFile { get; set; }

        protected ChannelMixSet channelMixSet;

        public Dictionary<string, float> ChannelValues { get; set; } = new();

        public override void _Ready()
        {
            var gdFile = new File();
            gdFile.Open(MixesFile, File.ModeFlags.Read);
            var content = gdFile.GetAsText();
            gdFile.Close();
            channelMixSet = TomletMain.To<ChannelMixSet>(content);
        }

        public override void _Process(float delta)
        {
            var newChannelValues = new Dictionary<string, float>();

            foreach (var mix in channelMixSet.Mixes)
            {
                float previousValue = 0;
                newChannelValues.TryGetValue(mix.OutputChannelName, out previousValue);

                var rawValue = SimInput.Manager.GetActionValue("aircraft/" + mix.InputChannelName);
                newChannelValues[mix.OutputChannelName] = mix.Apply(rawValue, previousValue, delta);
            }

            ChannelValues = newChannelValues.ToDictionary(kvp => kvp.Key, kvp => Mathf.Clamp(kvp.Value, -1, 1));
        }
    }
}