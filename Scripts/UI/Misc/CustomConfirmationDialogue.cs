using Godot;
using System;

namespace UI.Misc;

public partial class CustomConfirmationDialogue : ConfirmationDialog
{
    private static readonly Vector2I size = new(300, 100);

    public void AskToConfirm()
    {
        Size = size;
        Title = "Please confirm...";
        PopupCentered();
    }
}