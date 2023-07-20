using Godot;

namespace Physics.Fluids;

public interface ISpatialFluid
{
    // 3D fluid, optionally with bounds 
    // All values passed in and out are in global space

    // Density of the fluid at any given point
    // Behaviour does not have to be defined if the point is not within the fluid.
    float DensityAtPoint(Vector3 point);

    // Whether the given point is within the fluid
    bool ContainsPoint(Vector3 point);

    // Nearest point on the boundary of the fluid. Can be undefined if this fluid does not have boundaries
    Vector3 BoundaryAtPoint(Vector3 point);

    // Velocity of the fluid flow at any given point
    Vector3 VelocityAtPoint(Vector3 point);

    // Calculate the normal vector of the surface closest to the given point
    Vector3 NormalAtPoint(Vector3 point);

    // Note: this a little complex. Even if a fluid is bounded, this value can be false.
    // What this actually signifies is whether it has boundaries that are meaningful to interact with
    public bool HasBoundaries { get; set; }
}