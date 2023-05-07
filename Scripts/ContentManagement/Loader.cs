using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Tomlet;

namespace ContentManagement
{
    public static class Loader
    {
        public static (List<Aircraft>, List<Location>) FindContent(string path)
        {
            var aircraftList = new List<Aircraft>();
            var locationList = new List<Location>();

            var files = Utils.GetItemsInDirectory(path, recursive: true)
                .Where(x => Utils.SplitAtExtension(x).Item2.ToLower() == "toml");

            var gdFile = new File();

            var fileMap = files.ToDictionary(path => path, path =>
            {
                gdFile.Open(path, File.ModeFlags.Read);
                var content = gdFile.GetAsText();
                gdFile.Close();
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
                    aircraft.LoadedFrom = filePath;
                    aircraft.NeedsLauncher = document.ContainsKey("aircraft.launcher");
                    aircraftList.Add(aircraft);
                }
                else if (document.ContainsKey("location"))
                {
                    var location = TomletMain.To<Location>(tomlString);
                    location.LoadedFrom = filePath;
                    locationList.Add(location);
                }
                else
                {
                    // Utils.LogError($"Failed parsing {filePath}: no aircraft or location section was present");
                }
            }

            aircraftList = aircraftList.OrderBy(x => x.Name).ToList();
            locationList = locationList.OrderBy(x => x.Name).ToList();

            return (aircraftList, locationList);
        }

        public static void SearchForAddOns(string path)
        {
            // Prevent loading addons when in editor.
            // That is because loading resource packs from editor causes the res:// tree to become deleted or something
            var isInEditor = !OS.HasFeature("standalone");


            Utils.GetItemsInDirectory(
                path, recursive: true)
                .ToList()
                .Where(x => Utils.SplitAtExtension(x).Item2.ToLower() == "pck")
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