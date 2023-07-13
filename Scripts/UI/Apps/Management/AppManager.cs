using Godot;
using System;
using System.Collections.Generic;

namespace UI.Apps.Management;

public partial class AppManager : Control
{
    // todo: this is a prototyping implementation. please add error checking and such

    [Export] private Control editButton;
    [Export] private Control editMenu;
    [Export] private OptionButton profileSelector;
    [Export] private ProfileNameSelector profileNameSelector;
    [Export] private Button deleteProfileButton;
    [Export] private Control appHolder;
    [Export] private NewAppSelector newAppSelector;

    private List<AppProfile> appProfiles;
    private AppProfile crntAppProfile;
    private Dictionary<AppLayoutInfo, NodePath> crntApps;
    private bool editing = false;

    public override void _Ready()
    {
        editMenu.Visible = false;
    }

    public void SetAvailableProfiles(List<AppProfile> newProfiles)
    {
        appProfiles = newProfiles;
        UpdateProfileSelector();
        SelectProfile(0);
    }

    private void UpdateProfileSelector()
    {
        profileSelector.Clear();
        foreach (var item in appProfiles)
        {
            profileSelector.AddItem(item.Name);
        }
    }

    private void SelectProfile(int index)
    {
        crntAppProfile = appProfiles[index];
        crntApps = new();

        deleteProfileButton.Disabled = crntAppProfile.IsDefault;

        foreach (var node in appHolder.GetChildren()) node.QueueFree();

        foreach (var appInfo in crntAppProfile.Apps)
        {
            InstanceApp(appInfo);
        }
    }

    private void InstanceApp(AppLayoutInfo appInfo)
    {
        // if this gets slow, add a scene cache
        var instance = ResourceLoader.Load<PackedScene>(appInfo.ScenePath).Instantiate<Misc.UserResize>();
        appHolder.AddChild(instance);

        instance.AnchorLeft = appInfo.AnchorLeft;
        instance.AnchorRight = appInfo.AnchorRight;
        instance.AnchorTop = appInfo.AnchorTop;
        instance.AnchorBottom = appInfo.AnchorBottom;

        instance.Size = appInfo.Size;
        instance.Position = appInfo.Position;

        crntApps[appInfo] = instance.GetPath();

        instance.Resizable = editing;
        instance.Movable = editing;
    }

    private void ToggleEdit()
    {
        editing = !editing;
        editButton.Visible = !editing;
        editMenu.Visible = editing;

        if (crntApps != null)
        {
            foreach (var nodePath in crntApps.Values)
            {
                var node = GetNode<Misc.UserResize>(nodePath);
                node.Resizable = editing;
                node.Movable = editing;
            }
        }
    }

    private void _on_AddProfile_pressed()
    {
        profileNameSelector.Popup();
    }

    private void _on_ProfileNameSelector_popup_hide()
    {
        if (profileNameSelector.SelectedName != null)
        {
            appProfiles.Add(new() { Name = profileNameSelector.SelectedName });
            UpdateProfileSelector();
            profileSelector.Select(appProfiles.Count - 1);
            SelectProfile(profileSelector.Selected);
        }
    }

    private void _on_DeleteProfile_pressed()
    {
        if (!crntAppProfile.IsDefault)
        {
            appProfiles.Remove(crntAppProfile);
            UpdateProfileSelector();
            profileSelector.Select(0);
            SelectProfile(0);
        }
    }

    private void _on_NewApp_pressed()
    {
        newAppSelector.Popup();
    }

    private void _on_NewAppSelector_popup_hide()
    {
        if (newAppSelector.SelectedApp == null) return;

        var appInfo = new AppLayoutInfo()
        {
            ScenePath = newAppSelector.SelectedApp.ScenePath,
            Size = new Vector2(200, 300), // todo: should add default size field
            Position = GetViewportRect().Size / 2 - new Vector2(100, 150),
            AnchorLeft = .5f,
            AnchorRight = .5f,
            AnchorTop = .5f,
            AnchorBottom = .5f,
        };

        InstanceApp(appInfo);

        crntAppProfile.Apps.Add(appInfo);
    }
}