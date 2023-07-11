# Planning of value setters

Value setters will be nodes that can be put in an aircraft (or potentially locations as well - can use an interface) to allow basic setting of things.

Example uses:
- disabling wheels when landing gear retracts
- applying settings from the flight config screen:
    - should it have reverse thrust
    - should it be the light or heavy version
    - should it have some sort of payload that can be dropped
    - should it have one wing or two

They may end up being turing complete, and it will probably be possible to do ridiculously complicated things with them, but that's ok. Some may argue that this is an instance of the inner platform effect, but I disagree because the runtime configurability of this system is essential and using code for this would be overkill.

This system could be created in various levels of complexity and capability, and it needs to be decided what level is appropriate. Do we need a type system, or can everything be one type, or can we use dynamic/duck typing?

It will rely heavily on resources, so it has to wait until godot fixes c# resource exports (which should happen soon hopefully, as they're working on fixing other c# things).

## Types of values settable through aircraft config, and their relevant fields

This is rather relevant to valuesetters, since they will be a lot of the sources

All have name and description
- numeric (default, min, max, unit type (eg length or mass), step/rounding)
- dropdown, selecting from a list of strings (default, available values)
- boolean (default)
- Lists maybe would be useful in some cases but that's way too complex, we'd end up reinventing the godot editor. so no
- vector could be nice but we don't really need that as it can be worked around easily with three numbers. in any case we can add it later easily since it's no different from numbers
- strings could be added but we don't need them

## Parts of the value setter

These are all exported resources. Everything is a dynamic within here, and that's just what happens when you have a runtime-defined system.


#### Source
Where the value comes from:
- Aircraft config
- Read property from another node

#### Update frequency
- on ready
- on aircraft reset
- process (every n frames)
- physics process (every n frames)
- on signal? (don't need this yet but wouldn't be hard to add)

#### Transformation
There is a list of these, each one uses the value of the previous. There's a lot (this is essentially a graphical programming language), so they've been nested here. The listed input types are only are the type that receives from above, it may have regular exports for other properties (eg it gets value from above and compares it to regular export).
- numeric comparison
    - equal to: num -> bool
    - greater than: num -> bool
    - less than: num -> bool
    - between: num -> bool
    - switch/case: num -> variant
- math:
    - add/sub/mult/div: num -> num
    - takefrom: num -> num (like sub but the left hand value is constant)
    - map: num -> num
    - sqrt: num -> num
    - topower: num -> num (this rases value to fixed power)
    - nthpower: num -> num (this raises fixed base to variable power)
- logic
    - and (supporting N values?): bool[] -> bool
    - or (supporting N values?): bool[] -> bool
    - not: bool -> bool
    - xor (supporting N values?): bool[] -> bool
- strings
    - equal to: string -> bool
    - equal to any: string -> bool
    - contains: string -> bool
    - length: string -> num
    - switch: string -> variant

Hold on, we could just make everything extend from a single thing and just have a recursive tree where some values are branches, some are sources and some are  (similar to blender shader). But I think that might end up making the "code" read backwards. And that's more capable than required. At that point you might as well implement scratch in godot's sidebar.

#### Output
- SingleOutput (target, property)