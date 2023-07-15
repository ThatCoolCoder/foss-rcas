using Godot;
using System;
using System.Collections.Generic;
using Tomlet.Attributes;

namespace ContentManagement;

public abstract partial class ContentItem
{
    [TomlProperty("name")] public string Name { get; set; } = "";

    [TomlProperty("author")] public string Author { get; set; } = "";

    [TomlProperty("version")] public string Version { get; set; } = "";
    [TomlProperty("date_created")] public DateTime DateCreated { get; set; } = DateTime.MinValue;
    [TomlProperty("date_updated")] public DateTime DateUpdated { get; set; } = DateTime.MinValue;

    [TomlProperty("description")] public string Description { get; set; } = "";

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

    public List<ContentProblem> FindProblems()
    {
        var isEmpty = (string x) => x.StripEdges().Length == 0;

        var problems = new List<ContentProblem>();
        if (isEmpty(Name)) problems.Add(new("Addon name is blank", ProblemType.Error));
        if (isEmpty(Author)) problems.Add(new("Author name is blank", ProblemType.Warning));
        if (isEmpty(Description)) problems.Add(new("No description was provided", ProblemType.Warning));

        problems.AddRange(InnerFindProblems());

        return problems;
    }

    protected abstract List<ContentProblem> InnerFindProblems();
}