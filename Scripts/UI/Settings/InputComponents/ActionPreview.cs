using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using SimInput;

namespace UI.Settings.InputComponents;

public partial class ActionPreview : Control
{
    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ActionPreview.tscn");

    private SimInput.InputAction action;
    private string actionPath;
    private HBoxContainer mappingHolder;
    private NewMappingPopup newMappingDialog;

    public ActionPreview Config(Node parent, SimInput.InputAction _action, string _actionPath)
    {
        action = _action;
        actionPath = _actionPath;
        if (parent != null) parent.AddChild(this);

        return this;
    }

    public override void _Ready()
    {
        var name = (action.DisplayName == "" ? action.Name : action.DisplayName).Capitalize();
        if (action.Description is not null or "") name += action.Description;
        GetNode<Label>("Label").Text = name;
        mappingHolder = GetNode<HBoxContainer>("HBoxContainer/MappingHolder");
        newMappingDialog = GetNode<NewMappingPopup>("NewMappingPopup");
        TooltipText = $"(internal name {action.Name})";

        UpdateMappings();

        SettingsScreen.OnSettingsChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged()
    {
        UpdateMappings();
    }

    private void UpdateMappings()
    {
        foreach (var child in mappingHolder.GetChildNodeList()) child.QueueFree();

        foreach (var mapping in SettingsScreen.NewSettings.InputMap.GetMappingsForAction(actionPath))
        {
            ControlMappingPreview.Scene.Instantiate<ControlMappingPreview>().Config(mappingHolder, mapping, DeleteControlMapping);
        }

    }

    public override void _ExitTree()
    {
        SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
    }

    private void DeleteControlMapping(SimInput.AbstractControlMapping mapping)
    {
        SettingsScreen.NewSettings.InputMap.GetMappingsForAction(actionPath).Remove(mapping);
        UpdateMappings();
    }

    private void _on_NewMappingButton_pressed()
    {
        newMappingDialog.PopupCentered();
    }

    private void _on_NewMappingDialog_popup_hide()
    {
        if (newMappingDialog.NewMapping == null) return;

        SettingsScreen.NewSettings.InputMap.GetMappingsForAction(actionPath).Add(newMappingDialog.NewMapping);
        UpdateMappings();

        mappingHolder.GetChildNodeList().Last().EmitSignal("pressed");
    }
}
