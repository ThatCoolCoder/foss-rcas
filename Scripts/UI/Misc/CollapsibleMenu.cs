using Godot;
using System;

namespace UI.Misc
{
    [Tool]
    public class CollapsibleMenu : Control
    {
        [Export] public int Spacing { get; set; }
        [Export]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                UpdateTitle();
            }
        }
        private string _title = "Collapsible";
        private Label titleLabel;
        private Button toggleButton;

        [Export]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (titleLabel != null) UpdateLayout();
                if (OnStateChange != null) OnStateChange(_isOpen);
            }
        }
        private bool _isOpen = false;

        public event Action<bool> OnStateChange;

        public override void _Ready()
        {
            titleLabel = GetNode<Label>("Header/HBoxContainer/Title");
            toggleButton = GetNode<Button>("Header/HBoxContainer/Button");
            UpdateTitle();
            UpdateLayout();
        }

        private void UpdateTitle()
        {
            if (titleLabel != null) titleLabel.Text = _title;
        }

        private void _on_Button_pressed()
        {
            IsOpen = !IsOpen;
        }

        protected void UpdateLayout()
        {
            toggleButton.Text = IsOpen ? "-" : "+";
            // Start at 1 so we never affect the header
            for (int i = 1; i < GetChildCount(); i++)
            {
                var child = GetChild<Control>(i);
                child.Visible = IsOpen;
            }
        }
    }
}