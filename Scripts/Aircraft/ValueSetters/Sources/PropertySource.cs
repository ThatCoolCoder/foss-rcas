using Godot;
using System;

namespace Aircraft.ValueSetters.Sources
{
    [GlobalClass]
    public partial class PropertySource : AbstractValueSource
    {
        [Export] public Node Node { get; set; }
        [Export] public string Property { get; set; }

        public override dynamic GetValue()
        {
            return Node.Get(Property);
        }
    }
}