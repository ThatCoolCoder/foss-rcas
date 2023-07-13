using Godot;
using System;
using System.Collections.Generic;

namespace UI.Apps.Management;

public partial class AppManager : Control
{
    // todo: this is a prototyping implementation. please add error checking and such

    [Export] public UIAppInfoList AvailableApps { get; set; }

    [Export] private Control editButton;
    [Export] private Control editMenu;
    [Export] private OptionButton profileSelector;
    [Export] private Button deleteProfileButton;
    [Export] private Control appHolder;

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

        profileSelector.Clear();
        foreach (var item in appProfiles)
        {
            profileSelector.AddItem(item.Name);
        }
        profileSelector.Select(0);
    }

    private void SelectProfile(int index)
    {
        crntAppProfile = appProfiles[index];
        crntApps = new();

        deleteProfileButton.Disabled = crntAppProfile.IsDefault;

        foreach (var node in appHolder.GetChildren()) node.QueueFree();

        foreach (var appInfo in crntAppProfile.Apps)
        {
            // if this gets slow, add a scene cache
            var instance = ResourceLoader.Load<PackedScene>(appInfo.ScenePath).Instantiate<Control>();
            appHolder.AddChild(instance);

            instance.AnchorLeft = appInfo.AnchorLeft;
            instance.AnchorRight = appInfo.AnchorRight;
            instance.AnchorTop = appInfo.AnchorTop;
            instance.AnchorBottom = appInfo.AnchorBottom;

            instance.Position = appInfo.Position;
            instance.Size = appInfo.Size;

            crntApps[appInfo] = instance.GetPath();
        }
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

    private void _on_DeleteProfile_pressed()
    {
        if (!crntAppProfile.IsDefault)
        {
            appProfiles.Remove(crntAppProfile);
            profileSelector.Select(0);
        }
    }
}