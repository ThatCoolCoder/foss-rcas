# Audio notes

The current audio system is quite lacking and truthfully was only ever intended as a preliminary implementation. The following aspects of audio need to be covered in the new system(s). If we add all of them we'll need to add settings to disable/voladjust many, partially for performance on terrible hw.

## Propeller (and ducted fan)

For smaller aircraft this is the most important and noticeable sound. Key properties are:
- it is quieter at low RPM
- pitch changes with RPM
- At high throttle settings the sound becomes harsh and almost crackly
    - This may actually be triggered by a ratio of forward speed to rpm (potentially caused by a stalled blade)

This could potentially be implemented by blending two clips - a regular and a "stalled". But the difficulty is in getting the sound to align with its very periodic nature  - otherwise there will be weird phase-shifting harmonics. Perhaps they would have to be created electronically or extracted from a single sample.

Could also incorporate grass-cutting noise when an object enters a slightly larger than normal trigger disc.

It looks like the momentum/torque management system might need to be revamped, it seems a bit weird keeping all that on the prop rather than the motor or a dedicated Shaft component.

## Brushless motor

This sound is fairly simple, probably a single whining sample changing pitch and volume work for all motors. Volume likely needs input from both RPM and throttle.

Whining should include a slight vibration probably.

## Internal combustion motor

This is the defining sound for aircraft that use them. This is quite difficult. The details of what input parameters are available won't be known until the engine simulation itself is complete. Likely individual cylinders firing won't be modelled, the stall will just be a rpm at which it doesn't run if starter is off. Key factors:

- Idle is uneven (this may be modelled for us in the engine sim)
- Speed of audio changes with rpm, but
- Volume changes
- There is also noise when the throttle is 0 and the engine is just turning, but that's not that important since propeller noise can cover it
- Although a starting noise might be wanted

The problems with getting the sounds to align are again present.

## Servo sounds

Nothing too fancy, just a thing that makes noise based on delta-angle. Would unfortunately be too difficult to tap into servo load, for they are modelled as infinitely strong.

## Wheel sounds

Not crucial but would be nice to have a bit of a thud when you touch down and a then a subtle rolling noise.

- Thud proportional to suspension force perhaps.
- Rolling proportional
- Skidding proportional to slip (include both left and right skid)
    - Would need to adjust skid noise based on hard/soft

## Impact noises

- Bonk/crack when 2 hard things collide
- Scraping noise for when hard things scrape hard things
- Potentially very subtle scrape for hard thing on soft things

Will require classification of the GLTF into soft and hard potentially.
Or a proper terrain solution could be used.

## Environmental noises

These do not need to be too difficult, with the exception of wind which may end up being procedurally generated. Just a loopable audio sample in an audiostreamplayer3d.

Examples:
- Water
- Birds
- Wind (possibly trees in wind as well)

## Water noises

When water is added, noises for moving in it will be needed. This can be handled somewhere near the forcers, for they are what interact with the fluids. I think perhaps all forcers should feel all fluids.

- Smack noise for hard impacts
- Planing noise not dissimilar to leaves rustling
- Prop would want a disrupted noise for when it's moving.

## Object types

A bunch of the stuff above needs to know some basic material properties of an object. Here are the possible types:
- Hard+solid (concrete, asphalt, wood, metal)
- Hard but loose (compacted dry dirt)
- Medium (bare dirt)
- Soft (grass, leaves)