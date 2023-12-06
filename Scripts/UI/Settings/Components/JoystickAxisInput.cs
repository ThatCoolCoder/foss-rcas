using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Settings.Components;

public record AxisId(JoyAxis Axis, int Device);

public partial class JoystickAxisInput : BaseInputInput<AxisId, AxisId>
{
    private Dictionary<AxisId, (float min, float max)> CandidateAxisValues = new();

    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/JoystickAxisInput.tscn");

    public new JoystickAxisInput Config(Node parent, string name, SettingReader<AxisId> read, SettingWriter<AxisId> write, string toolTip = "")
    {
        base.Config(parent, name, read, write, toolTip);

        return this;
    }

    protected override void OnInputWhenOpen(InputEvent _event)
    {
        if (_event is InputEventJoypadMotion joypadMotionEvent)
        {
            var id = new AxisId(joypadMotionEvent.Axis, joypadMotionEvent.Device);
            if (!CandidateAxisValues.ContainsKey(id))
            {
                CandidateAxisValues[id] = new();
            }

            var valueTuple = CandidateAxisValues[id];

            if (joypadMotionEvent.AxisValue < valueTuple.min) valueTuple.min = joypadMotionEvent.AxisValue;
            if (joypadMotionEvent.AxisValue > valueTuple.max) valueTuple.max = joypadMotionEvent.AxisValue;

            CandidateAxisValues[id] = valueTuple;

            UpdatePopup();
        }
    }

    protected override AxisId? GetCandidateValue()
    {
        // Get the axis that is currently the most moved (and therefore the current candidate)
        // returns null if no axis has been moved enough

        return CandidateAxisValues
            .Select(x => (id: x.Key, delta: x.Value.max - x.Value.min))
            .Where(x => x.Item2 > 0.5f)
            .OrderByDescending(x => x.Item2)
            .Select(x => x.id)
            .FirstOrDefault();
    }

    protected override string GetPopupText()
    {
        var candidate = GetCandidateValue();

        if (candidate == null) return "Move an axis to select it...";
        else return $"Axis {(int)candidate.Axis} (device {candidate.Device}) selected";
    }

    protected override string GetCurrentValueText()
    {
        var val = read(SettingsScreen.NewSettings);
        return $"Axis {(int)val.Axis} (device {val.Device})";
    }

    protected override void ClearCandidateValue()
    {
        CandidateAxisValues = new();
    }
}