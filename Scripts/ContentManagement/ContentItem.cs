using Godot;
using System;
using System.Collections.Generic;
using Tomlet.Attributes;

namespace ContentManagement;

public partial class ContentItem
{
    [TomlProperty("name")] public string Name { get; set; } = "";

    [TomlProperty("author")] public string Author { get; set; } = "";

    [TomlProperty("version")] public string Version { get; set; } = "";
    [TomlProperty("date_created")] public DateTime DateCreated { get; set; } = DateTime.MinValue;
    [TomlProperty("date_updated")] public DateTime DateUpdated { get; set; } = DateTime.MinValue;

    [TomlProperty("description")] public string Description { get; set; } = "";

    [TomlProperty("credits")] public string Credits { get; set; } = "";

    // file it was loaded from, without extension
    [TomlNonSerialized] public string LoadedFromWithoutExtension { get; set; } = "";

    public string GetThumbnailPath()
    {
        return LoadedFromWithoutExtension + ".png";
    }

    public string GetScenePath()
    {
        return LoadedFromWithoutExtension + ".tscn";
    }
}