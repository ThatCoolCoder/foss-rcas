using Godot;
using System;

namespace UI.Misc
{
    public class CustomConfirmationDialogue : ConfirmationDialog
    {
        private static readonly Vector2 size = new(300, 100);

        public void AskToConfirm()
        {
            RectSize = size;
            WindowTitle = "Please confirm...";
            PopupCentered();
        }
    }
}