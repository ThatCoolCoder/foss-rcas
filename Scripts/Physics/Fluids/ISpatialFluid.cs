using Godot;

namespace Physics.Fluids;

public interface ISpatialFluid
{
    // 3D fluid, optionally with bounds 
    // All values passed in and out should be in global space

    // Density of the fluid at any given point - generally should get higher as point is lower
    // Behaviour does not have to be defined if the point is not within the fluid.
    float DensityAtPoint(Vector3 point);

    // Whether the given point is within the fluid
    bool ContainsPoint(Vector3 point);

    // Velocity of the fluid flow at any given point
    Vector3 VelocityAtPoint(Vector3 point);

    // Calculate the normal vector of the surface closest to the given point
    Vector3 NormalAtPoint(Vector3 point);

    public FluidType Type { get; set; }
}