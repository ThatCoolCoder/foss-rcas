using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SimInput;

public partial class Manager : Node
{
    public static Manager Instance { get; private set; }

    // Maps of action path to action value
    private Dictionary<string, float> actionValues = new();
    private Dictionary<string, float> intermediateTimeActionValues = new(); // for when they're not previous yet but also not new
    private Dictionary<string, float> previousActionValues = new();
    private List<string> actionsStillOnDefaultValue = new(); // todo: investigate if a hashset is better suited for this

    private Dictionary<string, List<IControlMapping>> mappings = new();
    private Dictionary<string, InputAction> actionLookup = new(); // thing for efficiency
    private InputMap inputMap = new();

    public override void _EnterTree()
    {
        ProcessMode = ProcessModeEnum.Always;
        Instance = this;
    }

    public override void _ExitTree()
    {
        Instance = null;
    }

    public void LoadInputMap(InputMap newInputMap)
    {
        // Migrate data from the input map into self and prepare for input getting

        actionValues = new();
        previousActionValues = new();
        actionLookup = new();
        inputMap = new();
        actionsStillOnDefaultValue = new();

        foreach (var category in AvailableInputActions.Categories)
        {
            if (category.Name.Contains('/'))
            {
                Utils.LogError($"Category name \"{category.Name}\" is invalid - slashes are not permitted");
                continue;
            }

            foreach (var action in category.Actions)
            {
                if (action.Name.Contains('/'))
                {
                    Utils.LogError($"Channel name \"{category.Name}\" is invalid - slashes are not permitted");
                    continue;
                }

                var actionPath = GenerateActionPath(category.Name, action.Name);

                // Set defaults
                actionValues[actionPath] = action.DefaultValue;
                previousActionValues[actionPath] = action.DefaultValue;
                actionsStillOnDefaultValue.Add(actionPath);

                actionLookup[actionPath] = action;

                // Actually migrate it across
                if (newInputMap.Mappings.TryGetValue(actionPath, out var newMappings))
                {
                    inputMap.Mappings[actionPath] = newMappings.ToList();
                }
            }
        }
    }

    private string GenerateActionPath(string category, string actionName)
    {
        return category + "/" + actionName;
    }

    public override void _UnhandledInput(InputEvent _event)
    {
        foreach (var actionPath in inputMap.Mappings.Keys)
        {
            var mappings = inputMap.Mappings[actionPath];
            var action = actionLookup[actionPath];

            foreach (var mapping in mappings)
            {
                // Read value
                if (mapping.ProcessEvent(_event) is float val)
                {
                    actionValues[actionPath] = val;
                    actionsStillOnDefaultValue.Remove(actionPath);

                    // apply extra mappings
                    foreach (var extraMapping in action.MapTo)
                    {
                        actionValues[extraMapping.Key] = extraMapping.Value(val);
                        actionsStillOnDefaultValue.Remove(extraMapping.Key);
                    }
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        // save the previous action values at the end of each frame
        intermediateTimeActionValues = new(actionValues);
        CallDeferred("SetPreviousActionValues");
    }

    private void SetPreviousActionValues()
    {
        previousActionValues = new(intermediateTimeActionValues);
    }

    // (we can't have static methods and instance methods with the same name so instance ones are suffixed with I)
    public float GetActionValueI(string actionPath)
    {
        try
        {
            return actionValues[actionPath];
        }
        catch (KeyNotFoundException)
        {
            Utils.LogError($"Unknown action: {actionPath}");
            return 0;
        }
    }

    public bool IsActionPressedI(string actionPath)
    {
        try
        {
            return actionValues[actionPath] > 0;
        }
        catch (KeyNotFoundException)
        {
            Utils.LogError($"Unknown action: {actionPath}");
            return false;
        }
    }
    public bool IsActionJustPressedI(string actionPath)
    {
        try
        {
            return actionValues[actionPath] > 0 && previousActionValues[actionPath] <= 0;
        }
        catch (KeyNotFoundException)
        {
            Utils.LogError($"Unknown action: {actionPath}");
            return false;
        }
    }
    public bool IsActionJustReleasedI(string actionPath)
    {
        try
        {
            return actionValues[actionPath] <= 0 && previousActionValues[actionPath] > 0;
        }
        catch (KeyNotFoundException)
        {
            Utils.LogError($"Unknown action: {actionPath}");
            return false;
        }
    }

    public bool HasActionBeenUsedI(string actionPath)
    {
        return !actionsStillOnDefaultValue.Contains(actionPath);
    }



    public static float GetActionValue(string actionPath)
    {
        return Instance.GetActionValueI(actionPath);
    }

    public static bool IsActionPressed(string actionPath)
    {
        return Instance.IsActionPressedI(actionPath);
    }

    public static bool IsActionJustPressed(string actionPath)
    {
        return Instance.IsActionJustPressedI(actionPath);
    }

    public static bool IsActionJustReleased(string actionPath)
    {
        return Instance.IsActionJustReleasedI(actionPath);
    }

    public static bool HasActionBeenUsed(string actionPath)
    {
        return Instance.HasActionBeenUsedI(actionPath);
    }

}