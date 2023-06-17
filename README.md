# foss-rcas

FOSS RC Aviation Simulator (in early development)

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

Much of the physics is based on the boat simulator in my `godot-learning` repository, but it's sufficiently diverged to the point where any form of shared library would not be practical.

All units are the base SI units unless explcitly stated so.

Within the propeller/motor subsystem, positive rotations are clockwise

In accordance with Godot convention, -z is forward. In Blender +y is forward.

A note on singletons+autoload in this project: there have been a few cases where autoload was desirable. Unfortunately, because this is C#, the automatic global variable feature is not available like in GDscript. So instead we use the singleton pattern, in which the instance is registered by the non-static autoloaded part of the class when it enters/exits tree.

## Todo:
- Physics/simulations
    - Make common methods in AbstractSpatialFluidForcer for getting relative & local velocity, there is no point having code for this in every derived class
    - improve wing physics: allow importing some better format of curves
        - stall is mushy, not sharp
        - planes feel too draggy, EG in real life the T28 carries a lot more energy and is difficult to get down, this one just mushes in
            - does it actually, though?
            - this may have changed with the new propeller simulation.
    - make location altitude actually change air pressure (not very useful, but why not?)
    - AeroObject: needs some way of toggling on off so things like landing gear can stop being draggy
        - also would be nice to be able to do this with AeroSurfaces
        - tie it to a control input? I'd rather be able to tie it to a servo
        - need some way of expressing conditions without creating a turing-complete language?
    - PropellerWithModel needs a way of fading between the two models instead of simply hiding/showing (hard to do because import from gltf)
    - Propeller should stop spinning when it hits something
        - Easy to do with an area, but the trouble is propeller doesn't have a scene so it's hard to add this area
    - Make a SimpleThruster which has a max thrust value and a max speed value
        - this will be used to make it easier for beginners to create planes
        - thrust decays from max at a standstill to 0 at the max speed
    - Electrics simulation
        - At low throttle settings, the prop feels more resistance than when it is at zero throttle
            - test scenario: fly at full throttle, cut the power, prop might briefly slow to below blur speed but then it speeds up again.
        - Motors might be recharging the battery or doing weird things when they freewheel
        - Add some sort of resistance from the motor when it's not powering, otherwise props spin forever
        - Check anticlockwise physics is working as intended
        - Consider moment of inertia of motor, as prop should spin up and down differently.
            - currently it has too much aero force to weight ratio
        - Make an electrics debug UI thing, so people can see how much thrust/rpm/current their stuff makes and tweak parameters accordingly.
        - Make prop drag based on raw force, not efficiency factor force. This makes it way easier to tune to have the right rpm, current and thrust.
            - Will require retuning all the planes
    - Internal combustion engine simulation
        - Should be easier than electrics especially now that all the propeller stuff is done.
        - Just have a rpm/torque curve, then throttle directly controls torque proportion and fuel consumption
    - Potentially create a simulation of FPV inteference - we make a raycast from viewing position to camera, and degrade based on how many intersections.
- Input
    - Because the input maps are completely stored in toml (even down to what channels exist), if we update the game the names of the channels will not update for people.
        - Same problem applies to the default values of the channels.
        - Can potentially code some sort of import process
    - Add an input debug UI thing
    - Add a preview for all the inputs so we can check direction etc without flying (requires modifications to SimInput.Manager so it can run with a custom inputmap instead of that in SimSettings.Current)
    - move the rest of the input to siminputmanager? (EG throw, reset)
    - Decide which direction the inputs should go in and switch those that go in counterintuitive directions (big breaking change if we don't do it now)
    - set the default channel mappings to work on a ps/xbox controller without configuration
    - Can't select a key like enter or space in the key input editor, since they press the close button.
        - The same problem applies to joystick button0
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
    - add created date to all the content (search git history)
    - Content manager tries to read `Mixes.toml` file and then gets annoyed because it is not a content file
    - thing has a heart attack if there is any issue at all in loading settings.
    - if there is a non-permitted class found when loading the input map, the entire game crashes. Instead it should just skip that item
        - problem: tomlet doesn't appreciate returning null from a converter function, which is the place where we do the checks
            - hacky solution: create an InvalidMappingType
- General refactoring/organizing
    - standardize: `Addon` vs `AddOn`
    - rename `ControlSurface` to `Servo` (because that's what it is)
    - move `Art/Common` -> `Art/Locations/Common`
    - Should spatialfluidrepository become an autoload singleton?
    - Make all "modules" (EG Propeller, BrushlessMotor, Battery) instanceable scenes? (instead of just scripts)
    - Make more things (forcers, etc) use Utils.GetNodeWithWarnings, and give that method a better name
    - Reorganize stuff, currently we have aircraft stuff strewn everywhere.
        - Maybe make a simulation directory/namespace, then within have electrics, fluid dynamics
        - But then where does all the control stuff go?
        - And what about all the cameras?
- Add support for flight computers/gyros extending from ControlHub and program a couple of types so that we can have quadcopters
- audio
    - procedural?
    - can link motor sound to an advanced motor simulation? (rpm, air disturbance factor, air disturbance shape)
- Content
    - Make a "showroom" map where you can take pictures of the planes for thumbnails (becuase rendering them in blender is difficult now that props are instanced scenes)
        - use this to make thumbnails for all the scenes, at the correct resolution
        - perhaps this is not actually needed, we could just make a free camera and move plane tool so people can do it in whatever map they want
    - Create an EDF with retracts and flaps
    - Create a bushplane about 1.1-1.3kg size
    - Mini 3d: increase control surface size in the model, make it fly more 3d
    - Make large oval decent (model some low-poly houses, put them or impostors of them all around, put more rows so it actually loooks like a place)
    - Do a remaster of the T28
    - Make somewhere fun to fly FPV, like a racecourse
    - Make a content repository and download system (that doesn't sound like a nightmare at all)
- Graphics
    - Somehow make grass not jump around when camera moves, only appear/disappear.
        - Perhaps can use a system where we make a grid of points, wiggle them, then discard those which are outside of the radius.
        - Would probably be a bit slow on really large patches but in that case we could use a chunk system internally to completely ignore points a certain distance away from camera
        - Perhaps just a check for each row + col to see if it will be within distance at any point.
    - need to get it running on old hardware (target: Intel HD 3000 on minimum 720p)
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
- Mod/content system
    - Make addon content go in `AddonContent/`
    - There is support for loading mods as pck files at runtime, but it is a bit of a pain to make these pck files.
    - Create a content version value that lets the game know if a mod is compatible.
        - Perhaps have a semantic versioning system with with the ability to use wildcards (similar to how npm dependencies are specified).
        - Requires the game knowing its version
- It's likely that godot won't include the aircraft metadata .toml files in export, so tell it that they're assets
    - this might already be done through the export presets
- make settings fileinput lineedit editable?
- add a way to configure wind in-game (likely requires large redesign of flightsettingsscreen - vertical tab menu with icons?)
- come up with a neater way to add the default aircraft cameras (currently they're just stuck on by FlightSettingsScreen)
    - it's tricky because we use the content loading metadata to get the plane size. Perhaps we need to decouple the aircraft info bit from the content bit
        - or we could just give them the content bit.