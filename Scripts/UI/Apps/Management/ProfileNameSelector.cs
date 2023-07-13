using Godot;
using System;
using System.Collections.Generic;

namespace UI.Apps.Management;

public partial class ProfileNameSelector : Popup
{
    [Export] private LineEdit lineEdit;
    public string SelectedName { get; private set; }

    private void _on_about_to_popup()
    {
        SelectedName = null;
    }

    private void _on_Cancel_pressed()
    {
        Hide();
    }

    private void _on_Add_pressed()
    {
        SelectedName = lineEdit.Text;
        if (SelectedName == "") SelectedName = null;
        Hide();
    }
}