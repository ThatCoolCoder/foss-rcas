# foss-rcas

FOSS RC Aviation Simulator (prototype)

Much of the physics is based on the boat simulator in `godot-learning`, but it's sufficiently diverged to the point where any form of shared library would be impracticable.

All units are the base SI units unless explcitly stated so.

A note on singletons+autoload in this project: there have been a few cases where autoload was desirable. Unfortuantely, because this is C#, the automatic global variable feature is not available like in GDscript. So instead we use the singleton pattern, in which the instance is registered by the non-static part of the class when it enters/exits tree.

Todo:
- improve wing physics: allow importing some better format of curves and have it so that the AOA curves are not stupidly symmetrical 
- add body drag
- fix motor/propeller physics (it's very underpowered at the moment)