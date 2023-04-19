# foss-rcas

FOSS RC Aviation Simulator (prototype)

## Controls

ctrl+d to toggle debug mode, r to reset plane, space to launch plane if it doesn't have wheels, ctrl+shift+r to restart entire simulator, f1 for screenshot, p to pause.

The main aircraft controls can be seen and configured through `Settings>Input Map`

## Misc info

Much of the physics is based on the boat simulator in `godot-learning`, but it's sufficiently diverged to the point where any form of shared library would not be practical.

All units are the base SI units unless explcitly stated so.

In accordance with Godot convention, -z is forward. In Blender +y is forward.

A note on singletons+autoload in this project: there have been a few cases where autoload was desirable. Unfortunately, because this is C#, the automatic global variable feature is not available like in GDscript. So instead we use the singleton pattern, in which the instance is registered by the non-static autoloaded part of the class when it enters/exits tree.

## Todo:
- Physics/simulations
    - Make common methods in AeroSurface for getting relative & local velocity, there is no point having code for this in every derived class
    - improve wing physics: allow importing some better format of curves
        - stall is mushy, not sharp
        - planes feel too draggy, EG in real life the T28 carries a lot more energy and is difficult to get down, this one just mushes in
            - does it actually, though?
    - make location altitude actually change air pressure (not very useful, but why not?)
    - AeroObject: needs some way of toggling on off so things like landing gear can stop being draggy
        - also would be nice to be able to do this with AeroSurfaces
        - tie it to a control input? I'd rather be able to tie it to a servo
        - need some way of expressing conditions without creating a turing-complete language?
    - PropellerWithModel needs a way of fading between the two models instead of simply hiding/showing (hard to do because import from gltf)
    - Electrics simulation
        - calculate current draw and make battery go flat
        - Migrate all models to this
    - Propeller simulation
        - Have a torque + momentum based system.
        - This should make it possible to have a counter-torque on the motor too.
        - Currently force is based only on area (diameter) and exit speed (pitch + rpm). It doesn't take into account num of blades. There needs to be some factor used for determining how to convert rpm + pitch into exit speed.
    - Internal combustion engine simulation
        - Create some sort of IMotor interface so that propellers don't care about what type of motor they're attached to
- Input
    - make f2 toggle UI
    - Add a preview for all the inputs so we can check direction etc without flying (requires modifications to SimInput.Manager so it can run with a custom inputmap instead of that in SimSettings.Current)
    - move the rest of the input to siminputmanager? (EG throw, reset)
    - Can't select a key like enter or space in the key input editor, since they press the close button.
    - Add support for dual/triple rates.
        - Somehow add support for this in the mixer. I'd rather not have to mix every channel separately on every plane, and I also don't want to add special-case channels.
        - Potentially this can be implemented as part of a flight modes system.
- Docs
    - Rewrite aircraft creation
    - Write about content creation
- Content manager tries to read `Mixes.toml` file and then gets annoyed because it is not a content file
- General refactoring/organizing
    - Should spatialfluidrepository become an autoload singleton?
    - Make all "modules" (EG Propeller, BrushlessMotor, Battery) instanceable scenes? (instead of just scripts)
    - Make more things (forcers, etc) use Utils.GetNodeWithWarnings, and give that method a better name
- Add support for flight computers/gyros extending from ControlHub and program a couple of types so that we can have quadcopters
- audio
    - procedural
    - can link motor sound to an advanced motor simulation? (rpm, air disturbance factor, air disturbance shape)
- Content
    - Create an EDF
    - Create a bushplane about 1.1-1.3kg size
    - Mini 3d: increase control surface size in the model, make it fly more 3d
- Graphics
    - need to get it running on old hardware (target: Intel HD 3000 on low 720p)
        - problem: this IGPU will not be supported if we upgrade to godot 4.
    - Settings for stuff like shadows, AA, AO
    - Come up with a setup for rendering to an intermediate viewport so we can render at lower resolution and upsample
        - Some people may not like this idea but I think it's great if you have a hi-res monitor but your GPU can't game like that.
        - (it's much simpler than adjusting the monitor resolution or whatever)
    - If FPS is terrible, have a pop up in the corner that tells you to change your graphics.
    - Update billboard exporter script + multimesh instancers + impostorsprite3d to have the option of normal maps on these (it looks a lot better)
    - Create more impostor pine pics so it's not always just the same one
        - Maybe even make some more trees so they're not all the same
- possibly rework physics system to support non-fluid effectors
- Even though the CG is apparently already really far back, the models don't fly tailheavy. If I move the CG further back, they become suddenly very tailheavy
    - Maybe their elevator is just stalling, the flying wing flies amazingly and the CG is probably a bit forward even.
- Make mod system
    - Scanning for aircraft files in other directories is already done
    - But packaging the scene files in such a way that all paths is relative is difficult
    - Also it's likely to be difficult to load GLTF at runtime. Might need to run a custom importer.
- It's likely that godot won't include the aircraft metadata .toml files in export, so tell it that they're assets
- make settings fileinput lineedit editable?
- add a way to configure wind in-game (likely requires large redesign of flightsettingsscreen - vertical tab menu with icons?)
