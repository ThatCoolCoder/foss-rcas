using Godot;
using System;
using System.Collections.Generic;

using SimInput;

namespace UI.Settings.InputComponents;

using Components;

public partial class ControlMappingPreview : Button
{
    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/ControlMappingPreview.tscn");

    private IControlMapping controlMapping;
    private Action<IControlMapping> deleteFunc;
    private static readonly Texture2D axisIcon = ResourceLoader.Load<Texture2D>("res://Art/Icons/joystick.png");
    private static readonly Texture2D buttonIcon = ResourceLoader.Load<Texture2D>("res://Art/Icons/button.png");
    private static readonly Texture2D keyboardIcon = ResourceLoader.Load<Texture2D>("res://Art/Icons/keyboard.png");

    public ControlMappingPreview Config(Node parent, SimInput.IControlMapping _controlMapping, Action<IControlMapping> _deleteFunc)
    {
        controlMapping = _controlMapping;
        deleteFunc = _deleteFunc;
        if (parent != null) parent.AddChild(this);

        return this;
    }

    public override void _Ready()
    {
        var (text, icon) = CreateButtonContent();
        Text = text;
        Icon = icon;
    }

    private (string, Texture2D) CreateButtonContent()
    {
        // todo: perhaps there is a better way to do this than a bunch of ifs?
        // Trouble with lambdas is we need a specific type in the lambda but a dict of lamdas would have unspecific type

        // todo: use icons for this instead of words
        if (controlMapping == null)
        {
            Utils.LogError("Control mapping is null", this);
            return ("Null", null);
        }
        if (controlMapping is AxisControlMapping am) return ($"{(int)am.Axis}", axisIcon);
        if (controlMapping is ButtonControlMapping bm) return ($"{(int)bm.ButtonIndex}", buttonIcon);
        if (controlMapping is SimpleKeyboardControlMapping skm)
        {
            return (OS.GetKeycodeString((Key)skm.KeyScancode), keyboardIcon);
        }
        if (controlMapping is ThreePosKeyboardControlMapping tpkm)
            return ($"{OS.GetKeycodeString((Key)tpkm.Key1Scancode)}, {OS.GetKeycodeString((Key)tpkm.Key2Scancode)}, {OS.GetKeycodeString((Key)tpkm.Key3Scancode)}",
                keyboardIcon);

        Utils.LogError($"Unknown control type: {controlMapping.GetType().FullName}", this);
        return ("?", null);
    }

    private BaseControlMappingEditor CreateEditor()
    {
        if (controlMapping == null) Utils.LogError("Control mapping is null", this);
        if (controlMapping is AxisControlMapping am) return AxisMappingEditor.Scene.Instantiate<AxisMappingEditor>().Config(this, am);
        if (controlMapping is ButtonControlMapping bm) return ButtonMappingEditor.Scene.Instantiate<ButtonMappingEditor>().Config(this, bm);
        if (controlMapping is SimpleKeyboardControlMapping skm)
            return SimpleKeyboardMappingEditor.Scene.Instantiate<SimpleKeyboardMappingEditor>().Config(this, skm);
        if (controlMapping is ThreePosKeyboardControlMapping tpkm)
            return ThreePosKeyboardMappingEditor.Scene.Instantiate<ThreePosKeyboardMappingEditor>().Config(this, tpkm);
        return null;
    }

    private void _on_ControlMappingPreview_pressed()
    {
        var editor = CreateEditor();
        editor.DeleteFunc = () => deleteFunc(controlMapping);
        editor.PopupCentered();
    }
}
