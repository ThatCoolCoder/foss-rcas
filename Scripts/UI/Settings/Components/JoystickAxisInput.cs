using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Settings.Components
{
    public class JoystickAxisInput : SettingsRow<int>
    {
        private Label currentValueLabel;
        private Label axisSelectInfo;
        private PopupDialog popupDialog;

        private Button selectAnotherAxisButton;
        private Button okButton;

        private Dictionary<int, (float min, float max)> CandidateAxisValues = new();

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/JoystickAxisInput.tscn");

        public new JoystickAxisInput Config(Node parent, string name, SettingReader<int> read, SettingWriter<int> write, string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);

            currentValueLabel = GetNode<Label>("HBoxContainer/CurrentValue");
            axisSelectInfo = GetNode<Label>("PopupDialog/MarginContainer/VBoxContainer/AxisSelectInfo");
            popupDialog = GetNode<PopupDialog>("PopupDialog");

            selectAnotherAxisButton = GetNode<Button>("PopupDialog/MarginContainer/VBoxContainer/HBoxContainer/SelectAnotherAxis");
            okButton = GetNode<Button>("PopupDialog/MarginContainer/VBoxContainer/HBoxContainer/Ok");

            return this;
        }

        public override void _Input(InputEvent _event)
        {
            if ((popupDialog?.Visible ?? false) && _event is InputEventJoypadMotion joypadMotionEvent)
            {
                if (!CandidateAxisValues.ContainsKey(joypadMotionEvent.Axis))
                {
                    CandidateAxisValues[joypadMotionEvent.Axis] = new();
                }

                var valueTuple = CandidateAxisValues[joypadMotionEvent.Axis];

                if (joypadMotionEvent.AxisValue < valueTuple.min) valueTuple.min = joypadMotionEvent.AxisValue;
                if (joypadMotionEvent.AxisValue > valueTuple.max) valueTuple.max = joypadMotionEvent.AxisValue;

                CandidateAxisValues[joypadMotionEvent.Axis] = valueTuple;

                UpdateAxisSelectInfo();
            }
        }

        private int? GetCandidateAxis()
        {
            // Get the axis that is currently the most moved (and therefore the current candidate)
            // returns null if no axis has been moved enough

            return CandidateAxisValues
                .Select(x => (x.Key, x.Value.max - x.Value.min))
                .Where(x => x.Item2 > 0.5f)
                .OrderByDescending(x => x.Item2)
                .Select(x => (int?)x.Item1)
                .FirstOrDefault();
        }

        private void UpdateAxisSelectInfo()
        {
            var candidate = GetCandidateAxis();

            if (candidate == null)
            {
                axisSelectInfo.Text = "Move an axis to select it...";
            }
            else
            {
                axisSelectInfo.Text = $"Axis {candidate} selected";
            }

            selectAnotherAxisButton.Visible = candidate != null;
            okButton.Visible = candidate != null;
        }

        protected override void OnSettingsChanged()
        {
            currentValueLabel.Text = $"Axis {read(SettingsScreen.NewSettings)}";
        }

        private void _on_ChangeAxisButton_pressed()
        {
            CandidateAxisValues.Clear();
            popupDialog.PopupCentered();
            UpdateAxisSelectInfo();
        }

        private void _on_Cancel_pressed()
        {
            popupDialog.Hide();
        }

        private void _on_SelectAnotherAxis_pressed()
        {
            CandidateAxisValues = new();
            UpdateAxisSelectInfo();
        }

        private void _on_Ok_pressed()
        {
            popupDialog.Hide();

            var axis = GetCandidateAxis();
            if (axis != null)
            {
                write(SettingsScreen.NewSettings, (int)axis);
            }
        }
    }
}