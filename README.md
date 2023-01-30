# foss-rcas

FOSS RC Aviation Simulator (prototype)

Much of the physics is based on the boat simulator in `godot-learning`, but it's sufficiently diverged to the point where any form of shared library would not be practical.

All units are the base SI units unless explcitly stated so.

In accordance with Godot convention, -z is forward. In Blender +y is forward.

A note on singletons+autoload in this project: there have been a few cases where autoload was desirable. Unfortuantely, because this is C#, the automatic global variable feature is not available like in GDscript. So instead we use the singleton pattern, in which the instance is registered by the non-static part of the class when it enters/exits tree.

Todo:
- improve wing physics: allow importing some better format of curves and have it so that the AOA curves are not stupidly symmetrical 
- add body drag
- add settings screen to configure various things
    - as part of the settings, remember what the last plane you flew was
    - add support for controls mixing and trimming
        - While users could configure this on their TXs, it seems nice to let them create a single generic sim model and have at least the mixing done automatically.
        - Plus, some people might be using gamepads or other input methods anyway.
- possibly rework physics system to support non-fluid effectors
- Even though the CG is apparently already really far back, the models feel nose heavy. If I move the CG further back, they don't become tailheavy, but they have weird rolling movements while pitching (asymmetric stall?)
- Should spatialfluidrepository become an autoload singleton?