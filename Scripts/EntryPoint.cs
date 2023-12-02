using System;
using System.Runtime.CompilerServices;

// Godot doesn't give us program.cs so we use this instead
public class EntryPoint
{
    [ModuleInitializer]
    public static void Run()
    {
        SimInput.AbstractControlMapping.CreateSubclassMappers();
        ContentManagement.AircraftConfigProperty.CreateSubclassMappers();
    }
}