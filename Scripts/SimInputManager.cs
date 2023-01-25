using Godot;
using System;
using System.Collections.Generic;

public class SimInputManager : Node
{
    // Godot's default input manager is woefully lacking in that it has no option to combine left/right movement of a stick into a single input.
    // So this is a simpler input manager that should be better for this use
    // Todo: add support for customisation of controls, and for more channels.
    // Possible todo: see if there is any way to get rid of this by writing a custom module for the regular input manager.

    private static SimInputManager instance;

    private Dictionary<int, float> axisValues = new();

    // mapping of action name to axis number
    public static Dictionary<string, int> AxisMapping = new()
    {
        {"aileron", 0},
        {"elevator", 1},
        {"throttle", 2},
        {"rudder", 3},
    };

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _ExitTree()
    {
        instance = null;
    }

    public override void _Input(InputEvent _event)
    {
        if (_event is InputEventJoypadMotion motionEvent)
        {
            var value = motionEvent.AxisValue;
            axisValues[motionEvent.Axis] = value;
        }
    }

    public static float GetAxisValue(int axisNumber)
    {
        if (instance == null) return 0;

        float result = 0;
        instance.axisValues.TryGetValue(axisNumber, out result);
        return result;
    }

    public static float GetAxisValue(string actionName)
    {
        try
        {
            return GetAxisValue(AxisMapping[actionName]);
        }
        catch (KeyNotFoundException)
        {
            GD.PrintErr($"Unknown action: {actionName}");
            return 0;
        }
    }
}