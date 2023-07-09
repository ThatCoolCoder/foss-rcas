using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Tomlet;

namespace ContentManagement
{
    public static class Loader
    {
        public static readonly string AddonContentDirectory = "res://AddonContent/";

        public static (List<Aircraft>, List<Location>) FindContent(string path)
        {
            var aircraftList = new List<Aircraft>();
            var locationList = new List<Location>();

            var files = Utils.GetItemsInDirectory(path, recursive: true)
                .Where(x => Utils.HasExtension(x, "content.toml"));

            var fileMap = files.ToDictionary(path => path, path =>
            {
                using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                var content = file.GetAsText();
                return content;
            });


            foreach (var pair in fileMap)
            {
                var filePath = pair.Key;
                var tomlString = pair.Value;

                var document = new TomlParser().Parse(tomlString);
                if (document.ContainsKey("aircraft"))
                {
                    var aircraft = TomletMain.To<Aircraft>(tomlString);
                    aircraft.LoadedFromWithoutExtension = filePath.ReplaceLast(".content.toml", "");
                    aircraft.NeedsLauncher = document.ContainsKey("aircraft.launcher");
                    aircraftList.Add(aircraft);
                }
                else if (document.ContainsKey("location"))
                {
                    var location = TomletMain.To<Location>(tomlString);
                    location.LoadedFromWithoutExtension = filePath.ReplaceLast(".content.toml", "");
                    locationList.Add(location);
                }
                else
                {
                    Utils.LogError($"Failed parsing {filePath}: no aircraft or location section was present");
                }
            }

            return (aircraftList, locationList);
        }

        public static void SearchForAddons(string path)
        {
            // Prevent loading addons when in editor.
            // That is because loading resource packs from editor causes the res:// tree to become deleted or something (it's weird)
            var isInEditor = !OS.HasFeature("standalone");

            Utils.GetItemsInDirectory(
                path, recursive: true)
                .ToList()
                .Where(x => Utils.HasExtension(x, "pck"))
                .ToList()
                .ForEach(x =>
                {
                    if (isInEditor) GD.Print($"Skipping addon pack {x} as this is an editor build");
                    else
                    {
                        GD.Print($"Loading addon pack {x}");
                        ProjectSettings.LoadResourcePack(x, replaceFiles: true);
                    }
                });
        }
    }
}