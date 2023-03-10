# foss-rcas

FOSS RC Aviation Simulator (prototype)

Much of the physics is based on the boat simulator in `godot-learning`, but it's sufficiently diverged to the point where any form of shared library would not be practical.

All units are the base SI units unless explcitly stated so.

In accordance with Godot convention, -z is forward. In Blender +y is forward.

A note on singletons+autoload in this project: there have been a few cases where autoload was desirable. Unfortunately, because this is C#, the automatic global variable feature is not available like in GDscript. So instead we use the singleton pattern, in which the instance is registered by the non-static autoloaded part of the class when it enters/exits tree.

ctrl+d to toggle debug mode, r to reset plane, space to launch plane, ctrl+shift+r to restart entire simulator.

Todo:
- screenshot date is not human-readable, fix this please
- improve wing physics: allow importing some better format of curves
    - stall is mushy, not sharp
    - planes feel too draggy, EG in real life the T28 carries a lot more energy and is difficult to get down, this one just mushes in
- add body drag
- make location altitude actually change air pressure (not very useful, but why not?)
- move the rest of the input to siminputmanager?
- Content manager tries to read `Mixes.toml` file and then gets annoyed because it is not a content file
- Add support for flight computers/gyros extending from ControlHub and program a couple of types so that we can have quadcopters
- audio
    - procedural
    - can link motor sound to an advanced motor simulation? (rpm, air disturbance factor)
- Graphics
    - need to get it running on old hardware (target: Intel HD 3000)
        - problem: this graphics will not be supported if we upgrade to godot 4.
    - add option for tree/grass multiplier, as even with impostors it can be intensive
- possibly rework physics system to support non-fluid effectors
- Even though the CG is apparently already really far back, the models don't fly tailheavy. If I move the CG further back, they become suddenly very tailheavy 
- Should spatialfluidrepository become an autoload singleton?
- Make mod system
    - Scanning for aircraft files in other directories is already done
    - But packaging the scene files in such a way that all paths is relative is difficult
    - Also it's likely to be difficult to load GLTF at runtime
- It's likely that godot won't include the aircraft metadata .toml files in export, so tell it that they're assets
- Make all "modules" instanceable scenes? (instead of just scripts)
- Electrics simulation to optionally simulate battery drain.
    - apply this to other engine types too
- make settings fileinput lineedit editable?
- add a way to configure wind in-game (likely requires large redesign of flightsettingsscreen - vertical tab menu with icons?)
