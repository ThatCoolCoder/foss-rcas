using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Settings.Components;

public partial class JoystickAxisInput : BaseInputInput<JoyAxis, JoyAxis?>
{
    private Dictionary<JoyAxis, (float min, float max)> CandidateAxisValues = new();

    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/JoystickAxisInput.tscn");

    public new JoystickAxisInput Config(Node parent, string name, SettingReader<JoyAxis> read, SettingWriter<JoyAxis> write, string toolTip = "")
    {
        base.Config(parent, name, read, write, toolTip);

        return this;
    }

    protected override void OnInputWhenOpen(InputEvent _event)
    {
        if (_event is InputEventJoypadMotion joypadMotionEvent)
        {
            if (!CandidateAxisValues.ContainsKey(joypadMotionEvent.Axis))
            {
                CandidateAxisValues[joypadMotionEvent.Axis] = new();
            }

            var valueTuple = CandidateAxisValues[joypadMotionEvent.Axis];

            if (joypadMotionEvent.AxisValue < valueTuple.min) valueTuple.min = joypadMotionEvent.AxisValue;
            if (joypadMotionEvent.AxisValue > valueTuple.max) valueTuple.max = joypadMotionEvent.AxisValue;

            CandidateAxisValues[joypadMotionEvent.Axis] = valueTuple;

            UpdatePopup();
        }
    }

    protected override JoyAxis? GetCandidateValue()
    {
        // Get the axis that is currently the most moved (and therefore the current candidate)
        // returns null if no axis has been moved enough

        return CandidateAxisValues
            .Select(x => (x.Key, x.Value.max - x.Value.min))
            .Where(x => x.Item2 > 0.5f)
            .OrderByDescending(x => x.Item2)
            .Select(x => (JoyAxis?)x.Item1)
            .FirstOrDefault();
    }

    protected override string GetPopupText()
    {
        var candidate = GetCandidateValue();

        if (candidate == null) return "Move an axis to select it...";
        else return $"Axis {(int)candidate} selected";
    }

    protected override string GetCurrentValueText()
    {
        return $"Axis {(int)read(SettingsScreen.NewSettings)}";
    }

    protected override void ClearCandidateValue()
    {
        CandidateAxisValues = new();
    }
}