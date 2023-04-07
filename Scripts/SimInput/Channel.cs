using Godot;
using System;
using System.Collections.Generic;

namespace SimInput
{
    public class Channel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float DefaultValue { get; set; } = 0; // change for things like throttle or flaps
        public List<IControlMapping> Mappings { get; set; } = new();

        public static Channel FromSingleMapping(string name, IControlMapping mapping, float defaultValue = 0, string description = "")
        {
            return new Channel()
            {
                Name = name,
                Description = description,
                Mappings = new() { mapping },
                DefaultValue = defaultValue
            };
        }
    }
}