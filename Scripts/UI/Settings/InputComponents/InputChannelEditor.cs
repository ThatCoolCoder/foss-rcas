using Godot;
using System;
using System.Collections.Generic;

namespace UI.Settings.InputComponents
{
    using SimInput;

    public class InputChannelEditor : Misc.CollapsibleMenu
    {
        private int channelIndex;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/InputChannelEditor.tscn");


        // yes an enum would be a cleaner solution. no, godot option buttons don't like enums and that is more effort anyway
        private Dictionary<string, Type> mappingTypes = new()
        {
            {"Joystick axis", typeof(AxisControlMapping)},
            {"Joystick button", typeof(ButtonControlMapping)},
            {"Keyboard (single key)", typeof(SimpleKeyboardControlMapping)},
            {"Keyboard (three position)", typeof(ThreePosKeyboardControlMapping)},
        };
        private OptionButton newMappingType;

        public InputChannelEditor Config(Node parent, string name, int _channelIndex)
        {
            if (parent != null) parent.AddChild(this);
            channelIndex = _channelIndex;

            Title = name;

            SettingsScreen.OnSettingsChanged += OnSettingsChanged;

            return this;
        }

        public override void _Ready()
        {
            newMappingType = GetNode<OptionButton>("MarginContainer/MaxSizeContainer/VBoxContainer/HBoxContainer/NewMappingType");

            newMappingType.Items.Clear();
            foreach (var mappingType in mappingTypes.Keys) newMappingType.AddItem(mappingType);

            base._Ready();
        }

        private void OnSettingsChanged()
        {
            UpdateChildren();
        }

        private void UpdateChildren()
        {
            var channel = SettingsScreen.NewSettings.InputMap.Channels[channelIndex];

            var holder = GetNode<Control>("MarginContainer/MaxSizeContainer/VBoxContainer/MappingList");

            foreach (var child in holder.GetChildNodeList()) child.QueueFree();

            foreach (var mapping in channel.Mappings)
            {
                ControlMappingPreview.Scene.Instance<ControlMappingPreview>().Config(holder, mapping, DeleteControlMapping);
            }
            UpdateLayout();
        }

        public override void _ExitTree()
        {
            SettingsScreen.OnSettingsChanged -= OnSettingsChanged;
        }

        private void _on_NewMappingButton_pressed()
        {
            var typeName = (string)newMappingType.GetSelectedItem();
            var mapping = (IControlMapping)Activator.CreateInstance(mappingTypes[typeName]);
            SettingsScreen.NewSettings.InputMap.Channels[channelIndex].Mappings.Add(mapping);
            SettingsScreen.ChangeSettings();
        }

        private void DeleteControlMapping(SimInput.IControlMapping mapping)
        {
            SettingsScreen.NewSettings.InputMap.Channels[channelIndex].Mappings.Remove(mapping);
            UpdateChildren();
        }
    }
}
