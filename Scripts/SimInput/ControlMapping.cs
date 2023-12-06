using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Tomlet;

namespace SimInput;

public abstract partial class AbstractControlMapping
{
    // Try to process event that is given. Return null if event isn't bound to the mapping 
    public abstract float? ProcessEvent(InputEvent _event);

    public static void CreateSubclassMappers()
    {
        Misc.TomletSubclassMapper.CreateMapping<AbstractControlMapping>(new Type[]
        {
            typeof(AxisControlMapping),
            typeof(ButtonControlMapping),
            typeof(KeyboardControlMapping),
        }, "MappingType");
    }
}

public partial class AxisControlMapping : AbstractControlMapping
{
    public uint Axis { get; set; }
    public float Multiplier { get; set; } = 1;
    public bool Inverted { get; set; } = false;
    public float DeadzoneEnd { get; set; } = 0.025f;
    public float DeadzoneRest { get; set; } = 0.025f;
    public int Device { get; set; } = 0;
    public bool AllDevices { get; set; } = false;

    public override float? ProcessEvent(InputEvent _event)
    {
        if (_event is InputEventJoypadMotion motionEvent && (uint)motionEvent.Axis == Axis && (motionEvent.Device == Device || AllDevices))
        {
            // Apply the mapping to the raw value
            // Inputs and outputs are in the -1 to 1 range

            float value = motionEvent.AxisValue;
            value = Mathf.Clamp(value * Multiplier, -1, 1);

            if (Inverted) value = -value;

            if (value < -1 + DeadzoneEnd) value = -1;
            if (value > 1 - DeadzoneEnd) value = 1;
            if (Mathf.Abs(value) < DeadzoneRest) value = 0;

            return value;
        }
        return null;
    }
}

public partial class ButtonControlMapping : AbstractControlMapping
{
    public uint ButtonIndex { get; set; }
    public bool Inverted { get; set; }
    public bool Momentary { get; set; } = true;
    private float currentValue = -1;
    public int Device { get; set; } = 0;
    public bool AllDevices { get; set; } = false;

    public override float? ProcessEvent(InputEvent _event)
    {
        if (_event is InputEventJoypadButton buttonEvent && (uint)buttonEvent.ButtonIndex == ButtonIndex && (buttonEvent.Device == Device || AllDevices))
        {
            // Apply the mapping to the raw value
            // Inputs and outputs are in the -1 to 1 range

            if (Momentary)
            {
                float value = buttonEvent.Pressed ? 1 : -1;
                if (Inverted) value = -value;
                return value;
            }
            else
            {
                if (buttonEvent.Pressed)
                {
                    currentValue = -currentValue;
                    return currentValue;
                }
            }
        }
        return null;
    }
}

public partial class KeyboardControlMapping : AbstractControlMapping
{
    // A note on this mapping: it supports both basic (single key) mappings,
    // as well as three-position mappings like you'd get from a 3 pos switch on a tx.
    // Ideally these would be in different classes but it turns out that it makes the UI a lot simpler
    // if there are just different types of this in which some fields are not used

    public uint KeyScancode { get; set; } = (uint)Key.A;
    [Tomlet.Attributes.TomlNonSerialized] public Key Key { set { KeyScancode = (uint)value; } }
    public uint Key2Scancode { get; set; }
    public uint Key3Scancode { get; set; }
    public MappingTypeEnum KeyboardMappingType { get; set; } = MappingTypeEnum.Momentary;
    private float currentToggleValue = -1;

    public override float? ProcessEvent(InputEvent _event)
    {
        if (_event is InputEventKey keyEvent && !keyEvent.Echo)
        {
            if (KeyboardMappingType == MappingTypeEnum.Momentary && (uint)keyEvent.GetKeycodeWithModifiers() == KeyScancode)
            {
                return keyEvent.Pressed ? 1 : -1;
            }
            else if (KeyboardMappingType == MappingTypeEnum.Toggle && (uint)keyEvent.GetKeycodeWithModifiers() == KeyScancode)
            {
                if (keyEvent.Pressed)
                {
                    currentToggleValue = -currentToggleValue;
                    return currentToggleValue;
                }
            }
            else if (KeyboardMappingType == MappingTypeEnum.ThreePosition && keyEvent.Pressed)
            {
                var scancode = (uint)keyEvent.GetKeycodeWithModifiers();
                if (scancode == KeyScancode) return -1;
                if (scancode == Key2Scancode) return 0;
                if (scancode == Key3Scancode) return 1;
            }
        }
        return null;
    }

    public enum MappingTypeEnum
    {
        Momentary,
        Toggle,
        ThreePosition
    }
}