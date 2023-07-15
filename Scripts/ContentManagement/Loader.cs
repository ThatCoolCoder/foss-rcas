using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Tomlet;

namespace ContentManagement;

public static class Loader
{
    public static readonly string AddonContentDirectory = "res://AddonContent/";

    public static (List<Aircraft> Aircraft, List<Location> Locations, Dictionary<string, List<ContentProblem>> Problems) FindContent(string path)
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

        var allProblems = new Dictionary<string, List<ContentProblem>>();

        foreach (var pair in fileMap)
        {
            var filePath = pair.Key;
            var tomlString = pair.Value;

            var crntProblems = new List<ContentProblem>();

            Tomlet.Models.TomlDocument document;
            try
            {
                document = new TomlParser().Parse(tomlString);
            }
            catch (Exception e)
            {
                crntProblems.Add(new($"Failed loading toml: {e.Message}", ProblemType.Error));
                goto SaveProblems;
            }

            var contentItemFindProblems = (ContentItem item) =>
            {
                var newProblems = item.FindProblems();
                crntProblems.AddRange(newProblems);
                return newProblems.Where(x => x.Type == ProblemType.Error).Count() > 0;
            };

            if (document.ContainsKey("aircraft"))
            {
                try
                {
                    var aircraft = TomletMain.To<Aircraft>(tomlString);
                    aircraft.LoadedFromWithoutExtension = filePath.ReplaceLast(".content.toml", "");
                    aircraft.NeedsLauncher = document.ContainsKey("aircraft.launcher");

                    if (contentItemFindProblems(aircraft)) goto SaveProblems;
                    aircraftList.Add(aircraft);
                }
                catch (Exception e)
                {
                    crntProblems.Add(new($"Failed parsing aircraft: {e.Message}", ProblemType.Error));
                    goto SaveProblems;
                }
            }
            else if (document.ContainsKey("location"))
            {
                try
                {
                    var location = TomletMain.To<Location>(tomlString);
                    location.LoadedFromWithoutExtension = filePath.ReplaceLast(".content.toml", "");

                    if (contentItemFindProblems(location)) goto SaveProblems;
                    locationList.Add(location);
                }
                catch (Exception e)
                {
                    crntProblems.Add(new($"Failed parsing location: {e.Message}", ProblemType.Error));
                    goto SaveProblems;
                }
            }
            else
            {
                crntProblems.Add(new($"No aircraft or location section was present", ProblemType.Error));
                goto SaveProblems;
            }

        SaveProblems:
            if (crntProblems.Count > 0) allProblems[filePath] = crntProblems;
        }

        return (aircraftList, locationList, allProblems);
    }

    public static void SearchForAddons(string path)
    {
        // Prevent loading addons when in editor.
        // That is because loading resource packs from editor causes the res:// tree to become deleted or something (it's weird)
        // todo: see if that was fixed in godot 4.0 (need to wait for godotpcktool to support the new pck format)
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