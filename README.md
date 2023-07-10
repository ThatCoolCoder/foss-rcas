# foss-rcas

FOSS RC Aviation Simulator (in early development)

This is the godot 4 conversion branch.

## Controls

- space to launch plane if it doesn't have wheels
- r to reset plane
- f1 to take a screenshot
- f2 to show/hide the UI when in flight
- p to pause.
- ctrl+d to toggle debug mode
- ctrl+shift+r to restart entire program

The main aircraft controls can be seen and configured through `Settings>Input Map`

## Misc info

The aerodynamics physics is derived from that in the boat simulator in my `godot-learning` repository, but it's sufficiently diverged to the point where any form of shared library would not be practical.

All units are the base SI units unless explcitly stated so. Radians are default.

Within the propeller/motor subsystem, positive rotations are clockwise

In accordance with Godot convention, -z is forward. In Blender +y is forward.

A note on singletons+autoload in this project: there have been a few cases where autoload was desirable. Unfortunately, because this is C#, the automatic global variable feature is not available like in GDscript. So instead we use the singleton pattern, in which the instance is registered by the non-static autoloaded part of the class when it enters/exits tree.

Channel directions:
- throttle: engine off is low
- aileron: stick left is low
- elevator: stick back is low
- rudder: stick left is low.

## Addons

It's not recommended to make or distribute addons currently as various formats haven't been stabilised yet. However, in future it's planned to create a site for hosting addons and possibly a downloader integrated into the game.

For help on installing addons, read [doesn't exist yet]

Information on creating addons can be found in [Docs/ContentCreation](Docs/ContentCreation/Index.md), although this information is incomplete and sometimes out of date.

## Contributing

Currently the project is still in early development. Things are constantly changing and because of that not much is written down yet. Therefore I invite you not to make contributions currently, as they may not align with my goals for this simulator.

## Guidelines

This section describes guidelines for simulator developers. For guidelines, tips and help on content creation, see [Docs/ContentCreation](Docs/ContentCreation/Index.md)

#### C#

This will be written at some later time, by this point the README document will be all cleaned up and this whole section will be moved to a separate document.

Discuss naming, whitespace + structure, namespaces, integration into godot, tools, 

#### Godot

Todo: write this section.

This section only describes stuff that you edit through the godot editor. Describe: naming, when to make a scene, whether to register scenes as nodes, 

#### Documentation

Todo: write this section

#### Git

Small commits and much use of feature branches are welcomed.

As there are many largish binary files (most of which are the base content), measures need to be taken to ensure that their history does not take up a ridiculous amount of space. Try to minimise editing of existing binary files. If creating a new content item or overhauling an existing one, switch to a new branch for that. Make your commits as required, then squash merge (`git merge --squash featurebranch`) and delete the feature branch. This means that only 1 set of changes is kept.

Depending on how much base content is desired and how large the repository gets, a more advanced solution such as git LFS might be used.

## Todo:
- Physics/simulations
    - Wheels
        - Make brakes and traction work, wheels spin constantly for no reason.
    - Make common methods in AbstractSpatialFluidForcer for getting relative & local velocity, there is no point having code for this in every derived class
    - improve wing/body aero physics
        - allow importing some better format of curves
            - xflr importer/exporter?
        - rudders are ineffective
            - are they?
        - dihedral too
        - stall is mushy, not sharp
            is it?
        - planes feel too draggy, EG in real life the T28 carries a lot more energy and is difficult to get down, this one just mushes in
            - does it actually, though?
            - this may have changed with the new propeller simulation.
    - make location altitude actually change air pressure (not very useful, but why not?)
    - AeroObject: needs some way of toggling on off so things like landing gear can stop being draggy
        - also would be nice to be able to do this with AeroSurfaces
        - tie it to a control input? I'd rather be able to tie it to a servo
        - need some way of expressing conditions without creating a turing-complete language?
    - PropellerWithModel needs a way of fading between the two models instead of simply hiding/showing (hard to do because import from gltf)
    - Electrics simulation
        - revamp motor sim to blend between power torque and stopping force instead of sudden cutoff
        - Check anticlockwise physics is working as intended
        - Consider moment of inertia of motor, as prop should spin up and down differently.
            - currently it has too much aero force to weight ratio
        - Make prop drag based on raw force, not efficiency factor force. This makes it way easier to tune to have the right rpm, current and thrust.
            - Will require retuning all the planes
    - Internal combustion engine simulation
        - Should be easier than electrics especially now that all the propeller stuff is done.
        - Just have a rpm/torque curve, then throttle directly controls torque proportion and fuel consumption
    - FPV
        - Redo fpv shader
            - remapping + clipping should only be done on brightness.
            - should have some sort of chromatic abberation
            - have hsv-space (or hsl-space) quantisation
        - Potentially create a simulation of FPV inteference - we make a raycast from viewing position to camera, and degrade based on how many intersections.
            - viewing position is FpvGroundStation, which is attached to the ground camera
    - prerecorded prop audio needs more control of volume
- Input
    - Add an input debug UI thing
    - Add a preview for all the inputs so we can check direction etc without flying (requires poking SimInput.Manager to give it a custom inputmap)
    - set the default controls to work on a ps/xbox controller without configuration
    - Um add a thing that checks the name of the controller so you can have different bindings for different controllers
        - hide bindings from controllers that are not present then, but have an option to show them so you can delete the unneeded ones
    - Add support for dual/triple rates.
        - Somehow add support for this in the mixer. I'd rather not have to mix every channel separately on every plane, and I also don't want to add special-case channels.
        - Potentially this can be implemented as part of a flight modes system.
- Docs
    - Rewrite aircraft creation
    - Write about content creation in general
    - Make content creation docs more friendly to non-programmers
    - If we make exotic stuff like quadcopters, create documentation on that
- Misc bugs/problems
    - GODOT BUG: it appears that [Exported] nodes aren't assigned if the exported is a child of a Window that starts out invisible.
    - make slow motion only affect aircraft, not cameras.
        - would be easy if time_scale was inherited like pause_mode
            - this was proposed but the engine developers decided against it
        - possibly integrate this into an aircraft-wide system involving also stuff like resetting batteries, etc.
            - propagate_call looks like it would make this easy, we won't even need interfaces
    - large oval has wrong ground normal
    - thing has a heart attack if it tries loading a content item that doesn't have a scene file
    - Anticlockwise propeller has bad shading due to the scaling by -1
    - settings toml file is technically incorrect with slashes in keys, should hopefully be a tomlet update to fix this soon.
    - Make propellerwithmodel properly stop spinning when it hits things
    - thing has a heart attack if there is any issue at all in loading settings.
    - if there is a non-permitted class found when loading the input map, the entire game crashes. Instead it should just skip that item
        - problem: tomlet doesn't appreciate returning null from a converter function, which is the place where we do the checks
            - hacky solution: create an InvalidMappingType
    - if it fails to find positions for spawn, game crashes
- General refactoring/organizing
    - if input settings screen becomes unperformant, make all the dialogs only be instanced when needed
    - move `Art/Common` -> `Art/Locations/Common`
    - Make all "modules" (EG Propeller, BrushlessMotor, Battery) instanceable scenes? (instead of just scripts)
    - Make more things (forcers, etc) use Utils.GetNodeWithWarnings, and give that method a better name
    - Reorganize stuff, currently we have aircraft stuff strewn everywhere.
        - Maybe make a simulation directory/namespace, then within have electrics, fluid dynamics
        - But then where does all the control stuff go?
        - And what about all the cameras?
- make gyros not suck
- audio
    - procedural?
    - can link motor sound to an advanced motor simulation? (rpm, air disturbance factor, air disturbance shape)
- UI
    - UI Apps system
        - Somewhat complex, we need:
            - app code itself
            - ui for creating apps, moving apps around, deciding which side they dock to, deleting apps
                - the docking part might be really easy because godot has the anchor property of controls designed for exactly this
            - code & format for saving + loading apps, and associated error checking
    - redo flightsettingscreen so that we can change stuff like spawn position and wind
        - total visual redesign, perhaps a vertical tab menu with icons
    - make a map thing that you can switch to and
        - teleport camera to location
        - move plane to predefined location
        - move plane to custom location (because why not, this is supposed to be free-ware and free includes freedom of it not blocking you)
- Content
    - create some super-light locations that are actually just 3d photos and some shadow catchers
        - (it appears some other sims do this)
    - Stabilise various formats, create compatibility fields
        - Thing saying what version of content file is used?
        - Thing saying what game versions are supported?
    - make thumbnails for all the scenes, at the correct resolution
    - Create an EDF with retracts and flaps
    - Create a bushplane about 1.1-1.3kg size
    - Mini 3d: increase control surface size in the model, make it fly more 3d, make it fly less trash
    - Make large oval decent (model some low-poly houses, put them or impostors of them all around, put more rows so it actually looks like a place)
    - Do a remaster of the T28
    - Make Ace's Track (not that large)
    - Make a converted golf course location (quite large)
    - Make a content repository and download system (that doesn't sound like a nightmare at all)
- Settings for what cameras are active and what order they switch through
    - requires addition of a camera ID property so that we can remember them
- Graphics
    - Grass overhaul again
        - Somehow make grass not jump around when camera moves, only appear/disappear.
            - Perhaps can use a system where we make a grid of points, wiggle them, then discard those which are outside of the radius.
            - Would probably be a bit slow on really large patches but in that case we could use a chunk system internally to completely ignore points a certain distance away from camera
            - Perhaps just a check for each row + col to see if it will be within distance at any point.
        - support for grass scatter on mesh
    - need to get it running on old hardware, and keep it running (target: Intel HD 3000 on minimum 720p)
    - Come up with a setup for rendering to an intermediate viewport so we can render at lower resolution and upsample
        - Some people may not like this idea but I think it's great if you have a hi-res monitor but your GPU can't game like that.
        - (it's much simpler than adjusting the monitor resolution or whatever)
    - If FPS is terrible, have a pop up in the corner that tells you to change your graphics.
    - Update billboard exporter script + multimesh instancers + impostorsprite3d to have the option of normal maps on these (it looks a lot better)
    - Maybe make some more pine trees so they're not all the same
- Even though the CG is apparently already really far back, the models don't fly tailheavy. If I move the CG further back, they become suddenly very tailheavy
    - Maybe their elevator is just stalling, the flying wing flies amazingly and the CG is probably a bit forward even.
- Mod/content system
    - There is support for loading mods as pck files at runtime, but it is a bit of a pain to make these pck files.
    - Create a content version value that lets the game know if a mod is compatible.
        - Perhaps have a semantic versioning system with with the ability to use wildcards (similar to how npm dependencies are specified).
        - Requires the game knowing its version
- make settings fileinput lineedit editable?
- update to godot 4?
    - wait until it has opengl support or whatever important thing it was missing