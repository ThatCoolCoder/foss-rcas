using Godot;
using System;
using System.Collections.Generic;

using SimInput;

namespace UI.Settings.InputComponents
{
    using Components;

    public class NewMappingDialog : WindowDialog
    {
        private Dictionary<string, Type> mappingTypes = new()
        {
            {"Joystick axis", typeof(AxisControlMapping)},
            {"Joystick button", typeof(ButtonControlMapping)},
            {"Keyboard (single key)", typeof(SimpleKeyboardControlMapping)},
            {"Keyboard (three position)", typeof(ThreePosKeyboardControlMapping)},
        };
        private OptionButton mappingTypeSelector;
        public Type SelectedMappingType { get; private set; }

        public override void _Ready()
        {
            mappingTypeSelector = GetNode<OptionButton>("MarginContainer/VBoxContainer/MappingTypeSelector");

            mappingTypeSelector.Items.Clear();
            foreach (var mappingType in mappingTypes.Keys) mappingTypeSelector.AddItem(mappingType);
            _on_MappingTypeSelector_item_selected(-1);

            base._Ready();
        }

        private void _on_MappingTypeSelector_item_selected(int _index)
        {
            SelectedMappingType = mappingTypes[(string)mappingTypeSelector.GetSelectedItem()];
        }

        private void _on_Cancel_pressed()
        {
            SelectedMappingType = null;
            Hide();
        }

        private void _on_Ok_pressed()
        {
            Hide();
        }
    }
}
