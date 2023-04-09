using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public static class OptionButtonExtensions
{
    public static object GetSelectedItem(this OptionButton optionButton)
    {
        return optionButton.GetItemText(optionButton.Selected);
    }
}