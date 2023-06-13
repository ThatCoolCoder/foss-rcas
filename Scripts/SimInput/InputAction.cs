using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public class InputAction
    {
        public string Name { get; set; }
        public string DisplayName { get; set; } // optional. Should not be capitalised, should be brief
        public string Description { get; set; } // information to be put after displayname if there is extra space
        public float DefaultValue { get; set; } = 0; // change for things like throttle or flaps
        public List<IControlMapping> Mappings { get; set; } = new();

        // Just some handy shortcuts
        public static InputAction FromSingleMapping(string name,
            IControlMapping mapping, float defaultValue = 0,
            string displayName = null, string description = null)
        {
            return new InputAction()
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                Mappings = new() { mapping },
                DefaultValue = defaultValue
            };
        }

        public static InputAction FromNoMapping(string name, float defaultValue = 0,
            string displayName = null, string description = null)
        {
            return new InputAction()
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                DefaultValue = defaultValue
            };
        }
    }
}