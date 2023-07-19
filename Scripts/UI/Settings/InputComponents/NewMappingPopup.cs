using Godot;
using System;
using System.Collections.Generic;

using SimInput;

namespace UI.Settings.InputComponents;

using Components;

public partial class NewMappingPopup : Window
{
    private SimInput.IControlMapping candidateNewMapping;
    public SimInput.IControlMapping NewMapping { get; private set; }
    private Dictionary<JoyAxis, (float min, float max)> axisValues { get; set; } = new();
    private const float minAxisTravel = 0.4f; // axis has to move this much to be registered, combats noisy controls
    private Label infoLabel;
    private Button selectAnotherInput;
    private Button ok;

    public override void _Ready()
    {
        infoLabel = GetNode<Label>("MarginContainer/VBoxContainer/InfoLabel");
        selectAnotherInput = GetNode<Button>("MarginContainer/VBoxContainer/Buttons/SelectAnotherInput");
        ok = GetNode<Button>("MarginContainer/VBoxContainer/Buttons/Ok");
    }

    private void _on_Cancel_pressed()
    {
        Hide();
    }

    private void _on_SelectAnotherInput_pressed()
    {
        candidateNewMapping = null;
        axisValues = new();
        UpdateControls();
    }

    private void _on_Ok_pressed()
    {
        NewMapping = candidateNewMapping;
        Hide();
    }

    private void _on_about_to_popup()
    {
        candidateNewMapping = null;
        axisValues = new();
        UpdateControls();
        NewMapping = null;
    }

    public override void _Input(InputEvent _event)
    {
        if (!Visible) return;

        if (_event is InputEventKey keyEvent)
        {
            candidateNewMapping = new SimInput.KeyboardControlMapping()
            {
                Key = keyEvent.GetKeycodeWithModifiers()
            };
            infoLabel.Text = OS.GetKeycodeString(keyEvent.GetKeycodeWithModifiers());
            UpdateControls();
        }
        else if (_event is InputEventJoypadMotion axisEvent)
        {
            if (!axisValues.ContainsKey(axisEvent.Axis))
            {
                axisValues[axisEvent.Axis] = new();
            }

            var valueTuple = axisValues[axisEvent.Axis];

            if (axisEvent.AxisValue < valueTuple.min) valueTuple.min = axisEvent.AxisValue;
            if (axisEvent.AxisValue > valueTuple.max) valueTuple.max = axisEvent.AxisValue;

            if (valueTuple.max - valueTuple.min > minAxisTravel)
            {
                candidateNewMapping = new SimInput.AxisControlMapping()
                {
                    Axis = (uint)axisEvent.Axis,
                };
                infoLabel.Text = $"Axis {(uint)axisEvent.Axis}";
                UpdateControls();
            }

        }
        else if (_event is InputEventJoypadButton buttonEvent)
        {

            candidateNewMapping = new SimInput.ButtonControlMapping()
            {
                ButtonIndex = (uint)buttonEvent.ButtonIndex
            };
            infoLabel.Text = $"Button {(uint)buttonEvent.ButtonIndex}";
            UpdateControls();
        }
    }

    private void UpdateControls()
    {
        if (candidateNewMapping == null)
        {
            ok.Visible = false;
            selectAnotherInput.Visible = false;
            infoLabel.Text = "Press/move an input to select it...";
        }
        else
        {
            ok.Visible = true;
            selectAnotherInput.Visible = true;
            // infoLabel.Text =
        }
    }
}