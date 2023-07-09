using Godot;
using System;
using Tomlet.Attributes;
using System.Collections.Generic;

namespace SimInput
{
    using MapperFuncList = Dictionary<string, Func<float, float>>;

    public partial class InputAction
    {
        public string Name { get; set; } = "";
        // todo: disp name and description shouldn't be serialized but we read this off the settings when doing UI so we need it
        public string DisplayName { get; set; } = ""; // optional. Should not be capitalised, should be brief
        public string Description { get; set; } = ""; // information to be put after displayname if there is extra space
        public float DefaultValue { get; set; } = 0; // change for things like throttle or flaps
        public MapperFuncList MapTo { get; set; } = new(); // allows mapping one action to another action - EG separated move forward and backward maps onto combined move. Should be a complete action path.

        public InputAction(string name, string displayName = null, string description = "", int defaultValue = 0, MapperFuncList mapTo = null)
        {
            Name = name;
            DisplayName = displayName ?? name;
            Description = description;
            DefaultValue = defaultValue;
            MapTo = mapTo ?? new();
        }
    }
}