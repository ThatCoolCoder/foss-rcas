using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Settings.Components;

public record ButtonId(JoyButton Button, int Device);

public partial class JoystickButtonInput : BaseInputInput<ButtonId, ButtonId>
{
    private ButtonId lastPressedButton;

    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/JoystickButtonInput.tscn");

    public new JoystickButtonInput Config(Node parent, string name, SettingReader<ButtonId> read, SettingWriter<ButtonId> write, string toolTip = "")
    {
        base.Config(parent, name, read, write, toolTip);

        return this;
    }

    protected override void OnInputWhenOpen(InputEvent _event)
    {
        if (_event is InputEventJoypadButton buttonEvent)
        {
            lastPressedButton = new(buttonEvent.ButtonIndex, buttonEvent.Device);
            UpdatePopup();
        }
    }

    protected override ButtonId GetCandidateValue()
    {
        // Get the axis that is currently the most moved (and therefore the current candidate)
        // returns null if no axis has been moved enough

        return lastPressedButton;
    }

    protected override string GetPopupText()
    {
        var candidate = GetCandidateValue();

        if (candidate != null) return $"Button {(int)candidate.Button} (device {candidate.Device}) selected";
        else return "Press a button on your controller to select it";
    }

    protected override string GetCurrentValueText()
    {
        var val = read(SettingsScreen.NewSettings);
        return $"Button {(int)val.Button} (device {val.Device})";
    }

    protected override void ClearCandidateValue()
    {
        lastPressedButton = null;
    }

    protected override bool ShouldShowSelectAgainButton()
    {
        return false;
    }
}