using Godot;
using System;

namespace ContentManagement;

public class ContentProblem
{
    public string Description { get; set; }
    public ProblemType Type { get; set; }

    public ContentProblem() { }
    public ContentProblem(string description, ProblemType type = ProblemType.Warning)
    {
        Description = description;
        Type = type;
    }
}

public enum ProblemType
{
    Error, // means that the mod is not ok at all
    Warning, // means that we should be able to load it but there are some deficiencies
}