using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{

    public class InputActionCategory
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = ""; // should not be capitalised
        public List<InputAction> Actions { get; set; } = new();

        public InputActionCategory()
        {

        }

        public InputActionCategory(string name, string displayName, List<InputAction> actions)
        {
            Name = name;
            DisplayName = displayName;
            Actions = actions;
        }
    }
}