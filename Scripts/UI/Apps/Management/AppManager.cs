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
    [Signal] public delegate void ChangesSavedEventHandler();

    private List<AppProfile> appProfiles;
    private AppProfile crntAppProfile;
    private Dictionary<NodePath, AppLayoutInfo> nodeToApp;
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
        nodeToApp = new();

        deleteProfileButton.Disabled = crntAppProfile.IsDefault;

        SaveCurrentLayout();
        foreach (var node in appHolder.GetChildren()) node.QueueFree();

        foreach (var appInfo in crntAppProfile.Apps)
        {
            InstanceApp(appInfo);
        }
    }

    private void SaveCurrentLayout()
    {
        foreach (var kvp in nodeToApp)
        {
            var node = GetNode<Control>(kvp.Key);
            var appInfo = kvp.Value;

            appInfo.AnchorLeft = node.AnchorLeft;
            appInfo.AnchorRight = node.AnchorRight;
            appInfo.AnchorTop = node.AnchorTop;
            appInfo.AnchorBottom = node.AnchorBottom;

            appInfo.SizeX = node.Size.X;
            appInfo.SizeY = node.Size.Y;
            appInfo.PositionX = node.Position.X;
            appInfo.PositionY = node.Position.Y;
        }


    }

    private void InstanceApp(AppLayoutInfo appInfo)
    {
        // if this gets slow, add a scene cache
        var instance = ResourceLoader.Load<PackedScene>(appInfo.ScenePath).Instantiate<Misc.UserManipulate>();
        appHolder.AddChild(instance);

        instance.AnchorLeft = appInfo.AnchorLeft;
        instance.AnchorRight = appInfo.AnchorRight;
        instance.AnchorTop = appInfo.AnchorTop;
        instance.AnchorBottom = appInfo.AnchorBottom;

        instance.Size = new Vector2(appInfo.SizeX, appInfo.SizeY);
        instance.Position = new Vector2(appInfo.PositionX, appInfo.PositionY);

        instance.AnchorLeft = appInfo.AnchorLeft;
        instance.AnchorRight = appInfo.AnchorRight;
        instance.AnchorTop = appInfo.AnchorTop;
        instance.AnchorBottom = appInfo.AnchorBottom;

        nodeToApp[instance.GetPath()] = appInfo;
        instance.OnDeleted += DeleteApp;

        instance.Resizable = editing;
        instance.Movable = editing;
        instance.Deletable = editing;
    }

    private void ToggleEdit()
    {
        editing = !editing;
        editButton.Visible = !editing;
        editMenu.Visible = editing;

        if (nodeToApp != null)
        {
            foreach (var nodePath in nodeToApp.Keys)
            {
                var node = GetNode<Misc.UserManipulate>(nodePath);
                node.Resizable = editing;
                node.Movable = editing;
                node.Deletable = editing;
            }
        }

        if (!editing)
        {
            SaveCurrentLayout();
            EmitSignal(SignalName.ChangesSaved);
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

        var position = GetViewportRect().Size / 2 - new Vector2(100, 150);

        var appInfo = new AppLayoutInfo()
        {
            ScenePath = newAppSelector.SelectedApp.ScenePath,
            SizeX = 200,
            SizeY = 300, // todo: should add default size field
            PositionX = position.X,
            PositionY = position.Y,
            AnchorLeft = .5f,
            AnchorRight = .5f,
            AnchorTop = .5f,
            AnchorBottom = .5f,
        };

        InstanceApp(appInfo);

        crntAppProfile.Apps.Add(appInfo);
    }

    private void DeleteApp(Misc.UserManipulate app)
    {
        var path = app.GetPath();
        var appLayoutInfo = nodeToApp[path];
        nodeToApp.Remove(path);

        crntAppProfile.Apps.Remove(appLayoutInfo);
    }
}