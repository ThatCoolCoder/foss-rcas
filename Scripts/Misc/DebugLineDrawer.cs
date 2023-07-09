using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class DebugLineDrawer : MeshInstance3D
{
    // Thing for drawing lines in 3D for debugging purposes (autoloaded)

    private Dictionary<(string, int), (Vector3, Vector3, Color)> lines = new();
    private StandardMaterial3D material = new StandardMaterial3D() { ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded, VertexColorUseAsAlbedo = true };

    private static DebugLineDrawer Instance;

    public void RegisterLine(Node node, Vector3 p1, Vector3 p2, Color? color = null, int lineId = 0)
    {
        lines[(node.GetPath().ToString(), lineId)] = (p1, p2, color ?? Colors.LightGray);
    }

    public static void RegisterLineStatic(Node node, Vector3 p1, Vector3 p2, Color? color = null, int lineId = 0)
    {
        if (Instance != null) Instance.RegisterLine(node, p1, p2, color, lineId);
    }

    public void ClearLine(Node node, int lineId = 0)
    {
        lines.Remove((node.GetPath().ToString(), lineId));
    }

    public static void ClearLineStatic(Node node, int lineId = 0)
    {
        if (Instance != null) Instance.ClearLine(node, lineId);
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
        if (Instance != null) Instance.ClearLines(node);
    }

    public override void _EnterTree()
    {
        Instance = this;
        Mesh = new ImmediateMesh();
    }

    public override void _ExitTree()
    {
        Instance = null;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var mesh = Mesh as ImmediateMesh;
        mesh.ClearSurfaces();

        if (lines.Count == 0) return;

        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

        foreach (var pair in lines.Values)
        {
            mesh.SurfaceSetColor(pair.Item3);
            mesh.SurfaceAddVertex(pair.Item1);
            mesh.SurfaceAddVertex(pair.Item2);
        }

        mesh.SurfaceEnd();
    }
}
