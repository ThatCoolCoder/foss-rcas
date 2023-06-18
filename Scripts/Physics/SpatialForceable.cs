using Godot;
using System;
using System.Collections.Generic;


namespace Physics
{
    public class SpatialForceable : RigidBody
    {
        // Spatial thing that can be forced
        // This class used to do more, now it is a bit empty but it still does a little so let's not get rid of it

        private List<Forcers.AbstractSpatialForcer> registeredForcers = new();

        public override void _IntegrateForces(PhysicsDirectBodyState state)
        {
            foreach (var forcer in registeredForcers)
            {
                if (forcer.Enabled)
                {
                    forcer.Apply(state);
                }
            }
        }

        public void RegisterForcer(Forcers.AbstractSpatialForcer forcer)
        {
            registeredForcers.Add(forcer);
        }

        public void UnregisterForcer(Forcers.AbstractSpatialForcer forcer)
        {
            registeredForcers.Remove(forcer);
        }

    }
}
