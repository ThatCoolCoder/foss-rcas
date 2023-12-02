using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UI.FlightSettings;

public partial class FlightSettingsScreen : Control
{
    [Export] private AircraftSelector aircraftSelector;
    [Export] private LocationSelector locationSelector;
    [Export] private Button failedLoadingInfoButton;
    [Export] private Popup failedLoadingPopup;
    [Export] private RichTextLabel failedLoadingInfo;
    private Dictionary<string, List<ContentManagement.ContentProblem>> contentLoadingProblems = new();

    public override void _Ready()
    {
        UpdateAvailableContent();

        // Try load the most recently used content
        TrySelectFromPath(SimSettings.Settings.Current.Misc.LastLoadedAircraft, aircraftSelector);
        TrySelectFromPath(SimSettings.Settings.Current.Misc.LastLoadedLocation, locationSelector);
    }

    private void UpdateAvailableContent()
    {
        ContentManagement.Loader.SearchForAddons(SimSettings.Settings.Current.Misc.AddonRepositoryPath);

        var (availableAircraft, availableLocations, problems) = ContentManagement.Loader.FindContent(ContentManagement.Loader.AddonContentDirectory);

        var aircraftScanResults = ContentManagement.Loader.FindContent(ContentManagement.Repositories.BaseAircraft);
        availableAircraft.AddRange(aircraftScanResults.Aircraft);
        foreach (var kvp in aircraftScanResults.Problems) problems[kvp.Key] = kvp.Value;

        var locationsScanResults = ContentManagement.Loader.FindContent(ContentManagement.Repositories.BaseLocations);
        availableLocations.AddRange(locationsScanResults.Locations);
        foreach (var kvp in locationsScanResults.Problems) problems[kvp.Key] = kvp.Value;

        aircraftSelector.AvailableItems = availableAircraft.OrderBy(x => x.Name).ToList();
        locationSelector.AvailableItems = availableLocations.OrderBy(x => x.Name).ToList();

        contentLoadingProblems = problems;
        failedLoadingInfoButton.Visible = contentLoadingProblems.Count > 0;
        var itemsTerm = Utils.Pluralize(problems.Count, "item", "items");
        failedLoadingInfoButton.Text = $"There were issues loading {problems.Count} {itemsTerm}. Click for more information.";
    }

    private void RememberLastLoadedContent()
    {
        SimSettings.Settings.Current.Misc.LastLoadedAircraft = aircraftSelector.SelectedItem.LoadedFromWithoutExtension;
        SimSettings.Settings.Current.Misc.LastLoadedLocation = locationSelector.SelectedItem.LoadedFromWithoutExtension;
        SimSettings.Settings.SaveCurrent();
    }

    private void TrySelectFromPath<T>(string lastLoadedPath, AbstractContentSelector<T> selector) where T : ContentManagement.ContentItem
    {
        // Try selecting the item that was loaded from a specific path

        var index = selector.AvailableItems.FindIndex(x => x.LoadedFromWithoutExtension == lastLoadedPath);
        if (index > 0) selector.SelectItem(index);
    }

    private void _on_BackButton_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/UI/StartScreen.tscn");
    }


    private void _on_PlayButton_pressed()
    {
        RememberLastLoadedContent();

        var location = ResourceLoader.Load<PackedScene>(locationSelector.SelectedItem.GetScenePath()).Instantiate<Locations.Location>();
        location.LocationInfo = locationSelector.SelectedItem;

        var aircraft = ResourceLoader.Load<PackedScene>(aircraftSelector.SelectedItem.GetScenePath()).Instantiate<Aircraft.Aircraft>();
        location.AircraftInfo = aircraftSelector.SelectedItem;
        aircraft.Config = aircraftSelector.GetAircraftConfig();

        location.AddChild(aircraft);
        location.Aircraft = aircraft;
        location.CrntSpawnPosition = locationSelector.SelectedItem.SpawnPositions.First();

        // switch scenes, done manually because we needed to set the values above
        var root = GetTree().Root;
        var tree = GetTree();
        root.RemoveChild(this);
        root.AddChild(location);
        tree.CurrentScene = location;
        QueueFree();
    }

    private void _on_FailedLoadingInfoButton_pressed()
    {
        var wereWas = Utils.Pluralize(contentLoadingProblems.Count(), "was", "were");
        var itemsTerm = Utils.Pluralize(contentLoadingProblems.Count(), "item", "items");
        var sections = new List<string>() { $"There {wereWas} {contentLoadingProblems.Count} {itemsTerm} with problems.\n" };
        foreach (var kvp in contentLoadingProblems)
        {
            var fileName = kvp.Key;
            var problemList = kvp.Value;

            var crntSection = $"[b]{fileName}[/b]\n";
            var errors = problemList.Where(x => x.Type == ContentManagement.ProblemType.Error);
            if (errors.Count() > 0)
            {
                var term = Utils.Pluralize(errors.Count(), "fatal error", "fatal errors");
                crntSection += $"{errors.Count()} {term}:\n";
                foreach (var error in errors) crntSection += $"- {error.Description}\n";
            }

            var warnings = problemList.Where(x => x.Type == ContentManagement.ProblemType.Warning);
            if (warnings.Count() > 0)
            {
                var term = Utils.Pluralize(warnings.Count(), "warning", "warnings");
                crntSection += $"{warnings.Count()} {term}:\n";
                foreach (var warning in warnings) crntSection += $"- {warning.Description}\n";
            }

            sections.Add(crntSection);
        }
        failedLoadingInfo.Text = String.Join("\n", sections);

        failedLoadingPopup.Popup();
        failedLoadingInfoButton.Visible = false;
    }

    private void _on_FailedLoadingInfoClose_pressed()
    {
        failedLoadingInfoButton.Visible = false;
    }
}