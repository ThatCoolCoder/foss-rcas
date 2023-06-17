using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    using MapperFuncList = Dictionary<string, Func<float, float>>;

    public class InputAction
    {
        public string Name { get; set; }
        [field: NonSerialized]
        public string DisplayName { get; set; } // optional. Should not be capitalised, should be brief
        [field: NonSerialized]
        public string Description { get; set; } // information to be put after displayname if there is extra space
        [field: NonSerialized]
        public float DefaultValue { get; set; } = 0; // change for things like throttle or flaps
        public List<IControlMapping> Mappings { get; set; } = new();
        [field: NonSerialized]
        public MapperFuncList MapTo { get; set; } = new(); // allows mapping one action to another action - EG separated move forward and backward maps onto combined move. Should be a complete action path.

        // Just some handy shortcuts
        public static InputAction FromSingleMapping(string name,
            IControlMapping mapping, float defaultValue = 0,
            string displayName = null, string description = null, MapperFuncList mapTo = null)
        {
            return new InputAction()
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                Mappings = new() { mapping },
                DefaultValue = defaultValue,
                MapTo = mapTo ?? new()
            };
        }

        public static InputAction FromNoMapping(string name, float defaultValue = 0,
            string displayName = null, string description = null, MapperFuncList mapTo = null)
        {
            return new InputAction()
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                DefaultValue = defaultValue,
                MapTo = mapTo ?? new()
            };
        }
    }
}