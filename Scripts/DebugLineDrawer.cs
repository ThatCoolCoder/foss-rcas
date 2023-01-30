using Godot;
using System.Collections.Generic;
using System.Linq;

class DebugLineDrawer : ImmediateGeometry
{
    // Thing for drawing lines in 3D for debugging purposes (autoloaded)

    private Dictionary<(string, int), (Vector3, Vector3, Color)> lines = new();

    private static DebugLineDrawer instance;

    public void RegisterLine(Node node, Vector3 p1, Vector3 p2, Color? color = null, int lineId = 0)
    {
        lines[(node.GetPath().ToString(), lineId)] = (p1, p2, color ?? Colors.LightGray);
    }

    public static void RegisterLineStatic(Node node, Vector3 p1, Vector3 p2, Color? color = null, int lineId = 0)
    {
        if (instance != null) instance.RegisterLine(node, p1, p2, color, lineId);
    }

    public void ClearLine(Node node, int lineId = 0)
    {
        lines.Remove((node.GetPath().ToString(), lineId));
    }

    public static void ClearLineStatic(Node node, int lineId = 0)
    {
        if (instance != null) instance.ClearLine(node, lineId);
    }

    public void ClearLines(Node node)
    {
        // Clear all lines for a given node
        var path = node.GetPath().ToString();
        lines = lines.Where(p => p.Key.Item1 != path).ToDictionary(p => p.Key, p => p.Value);
    }

    public static void ClearLinesStatic(Node node)
    {
        // Clear all lines for a given node
        if (instance != null) instance.ClearLines(node);
    }

    public override void _EnterTree()
    {
        instance = this;
        MaterialOverride = new SpatialMaterial() { FlagsUnshaded = true, VertexColorUseAsAlbedo = true };
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
            SetColor(pair.Item3);
            AddVertex(pair.Item1);
            AddVertex(pair.Item2);
        }

        End();
    }
}