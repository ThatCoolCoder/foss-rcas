using Godot;
using System;
using System.Collections.Generic;

namespace UI.Settings.Components;

public partial class EnumInput : SettingsRow<object>
{
    // Dropdown input for enum values.
    // Since you (fairly enough) can't have a node with generics, we have to throw away the type system and users will have to cast results from object.

    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/EnumInput.tscn");

    private OptionButton optionButton;
    private List<object> items = new();

    public EnumInput Config(Node parent, string name, SettingReader<object> read, SettingWriter<object> write,
        Type enumType, Func<object, string> customValueFormatter = null, string toolTip = "", List<object> permittedItems = null)
    {
        base.Config(parent, name, read, write, toolTip);

        if (!enumType.IsEnum) Utils.LogError($"{enumType.Name} is not an enum", this);

        optionButton = GetNode<OptionButton>("OptionButton");
        optionButton.Clear();
        foreach (var v in Enum.GetValues(enumType))
        {
            if (permittedItems != null && !permittedItems.Contains(v)) continue;
            optionButton.AddItem(customValueFormatter == null ? v.ToString() : customValueFormatter(v));
            items.Add(v);
        }

        return this;
    }

    public override void OnSettingsChanged()
    {
        optionButton.Select(items.IndexOf(read(SettingsScreen.NewSettings)));
        // _on_OptionButton_item_selected
    }

    private void _on_OptionButton_item_selected(int index)
    {
        write(SettingsScreen.NewSettings, items[index]);
    }
}