using Godot;
using System;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 vec, float x) => new Vector3(x, vec.Y, vec.Z);
    public static Vector3 WithY(this Vector3 vec, float y) => new Vector3(vec.X, y, vec.Z);
    public static Vector3 WithZ(this Vector3 vec, float z) => new Vector3(vec.X, vec.Y, z);

    public static Vector3 Random(Vector3 min, Vector3 max)
    {
        return new Vector3(
            (float)GD.RandRange(min.X, max.X),
            (float)GD.RandRange(min.Y, max.Y),
            (float)GD.RandRange(min.Z, max.Z)
        );
    }
}