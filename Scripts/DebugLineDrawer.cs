using Godot;
using System.Collections.Generic;
using System.Linq;

class DebugLineDrawer : ImmediateGeometry
{
    // Thing for drawing lines in 3D for debugging purposes (autoloaded)

    private Dictionary<(string, int), (Vector3, Vector3)> lines = new();

    private static DebugLineDrawer instance;

    public void RegisterLine(Node node, Vector3 p1, Vector3 p2, int lineId = 0)
    {
        lines[(node.GetPath().ToString(), lineId)] = (p1, p2);
    }

    public static void RegisterLineStatic(Node node, Vector3 p1, Vector3 p2, int lineId = 0)
    {
        if (instance != null) instance.RegisterLine(node, p1, p2, lineId);
    }

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _ExitTree()
    {
        instance = null;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        Clear();

        Begin(Mesh.PrimitiveType.Lines);

        foreach (var pair in lines.Values)
        {
            AddVertex(pair.Item1);
            AddVertex(pair.Item2);
        }

        End();
    }
}