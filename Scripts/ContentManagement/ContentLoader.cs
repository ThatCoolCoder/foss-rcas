using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Tomlet;

namespace ContentManagement
{
    public static class ContentLoader
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

            var parser = new TomlParser();
            foreach (var pair in fileMap)
            {
                var filePath = pair.Key;
                var tomlString = pair.Value;

                var document = parser.Parse(tomlString);
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
                    GD.PrintErr($"Error parsing {filePath}: no aircraft or location section was present");
                }
            }

            aircraftList = aircraftList.OrderBy(x => x.Name).ToList();
            locationList = locationList.OrderBy(x => x.Name).ToList();

            return (aircraftList, locationList);
        }
    }
}