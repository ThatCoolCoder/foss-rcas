using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SimInput
{
    public class Manager : Node
    {
        private static Manager Instance;

        // Map of action path to action value
        private Dictionary<string, float> actionValues = new();
        private Dictionary<string, float> previousActionValues = new();

        private Dictionary<string, InputAction> actionLookup = new(); // thing for efficiency
        private InputMap inputMap = new();

        public override void _EnterTree()
        {
            Instance = this;
        }

        public override void _ExitTree()
        {
            Instance = null;
        }

        public void LoadInputMap(InputMap newInputMap)
        {
            // Migrate data from the input map into self and prepare for input getting

            inputMap = new InputMap();
            actionLookup = new();
            actionValues = new();
            previousActionValues = new();

            foreach (var category in inputMap.ActionCategories)
            {
                var categoryName = category.Key;

                if (categoryName.Contains('/'))
                {
                    Utils.LogError($"Category name \"{categoryName}\" is invalid - slashes are not permitted");
                    continue;
                }

                var newCategory = new List<InputAction>();
                newInputMap.ActionCategories.TryGetValue(categoryName, out newCategory);

                foreach (var action in category.Value)
                {
                    if (action.Name.Contains('/'))
                    {
                        Utils.LogError($"Channel name \"{categoryName}\" is invalid - slashes are not permitted");
                        continue;
                    }

                    // Actually migrate it across
                    category.Value.Clear();
                    var newAction = newCategory.FirstOrDefault(a => a.Name == action.Name);
                    if (newAction != null) action.Mappings = newAction.Mappings.ToList();

                    var actionPath = GenerateActionPath(categoryName, action.Name);
                    actionLookup[actionPath] = action;

                    // Set defaults
                    actionValues[actionPath] = action.DefaultValue;
                    previousActionValues[actionPath] = action.DefaultValue;
                }
            }
        }

        private string GenerateActionPath(string category, string actionName)
        {
            return category + "/" + actionName;
        }


        public override void _Input(InputEvent _event)
        {
            foreach (var actionName in actionLookup.Keys)
            {
                var action = actionLookup[actionName];

                foreach (var mapping in action.Mappings)
                {
                    // Read value
                    if (mapping.ProcessEvent(_event) is float val) actionValues[action.Name] = val;
                }
            }
        }

        public override void _Process(float delta)
        {
            // save the previous action values at the end of each frame
            // todo: let's see if this actually works
            SetDeferred("previousActionValues", actionValues.ToList());
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

        public bool ActionIsPressedI(string actionPath)
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
        public bool ActionIsJustPressedI(string actionPath)
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
        public bool ActionIsJustReleasedI(string actionPath)
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

        public static float GetActionValue(string actionPath)
        {
            return Instance.GetActionValueI(actionPath);
        }

        public static bool ActionIsPressed(string actionPath)
        {
            return Instance.ActionIsPressedI(actionPath);
        }

        public static bool ActionIsJustPressed(string actionPath)
        {
            return Instance.ActionIsJustPressedI(actionPath);
        }

        public static bool ActionIsJustReleased(string actionPath)
        {
            return Instance.ActionIsJustReleasedI(actionPath);
        }

    }
}