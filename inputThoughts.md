## planning for redoing input

I think we need to migrate all of the input into SimInput, which requires some fiddling around.

At the same time, we should fix the issue of the input channels being completely defined by the toml. Solution: have SimInput.Manager read the input map (either from settings or a custom one) when it is started and migrate from that into an internal one? Or we could have a separate inputmap class that's the one that's actually persisted and do some messing hmm still seems hard.

## Channels
(not necessarily a channel in the sense of a channel on a plane, just a channel of input)

uuh so to support having a controller stick mapped to both directions or a key mapped to each we can create combined and separate bindings just like beamng.

yes but how do we combine them?
the combined one would be master obviously
maybe can add a proxychannel class which extends it and applies a lambda to the value to return a custom value.

hmm let's just not do that yet

hey the categories should have display names too
uuh that's a nightmare I guess we need to make something extending a dictionary

or maybe the things can just listen to multiple
since it's going to be all non-aircraft anyway

should channels have display names and the other names should be variable-like?
yeah

- Aircraft
    aileron
    elevator
    throttle
    rudder
    aux1 (gear) - why was this not called aux previously?
    aux2 (flap)
    aux3 (bomb)
    aux4 (spoilers)

- Camera
    forward/back
    forward
    back

    left/right
    left
    right

    up/down
    up
    down
    
    pan left/pan right
    pan left
    pan right

    tilt up/tilt down
    tilt up
    tilt down
    
- Gameplay
    launch
    reset
    pause

- Other
    none yet but good to have a category

## Data structure

Hmm so we have an inputmap class. The final channels should be stored as a dict so that they can be queried by runtime-defind strings. But it would be possible to have a class with some hard coded dicts since the category should be defind at compile-time. (aircraft shouldn't be allowed to look at a camera keybind, for instance)

HOLD UP
we have to differentiate between storage of the mappings and storage of the current channel values. I don't want to have to duplicate the channel categories and manually copy over all the values.

uuh maybe we just have a nested dict of channels and a flat dict of values but with a `/` to act as a separator (because it will be a pain passing around composite values for channels through godot ui, toml and c#)

#### proto 1

channels storage
```
class IMap {
    dict<string, List<Channel>> channels
}
class Channel {
    name
    description
    default
    List<bindings> bindings (also known as mappings)
}
```

channels migration
```
have template/default map
load saved map
have empty dict of string to channel, this is a lookup for later
for each category of the template:
    for each channel:
        check the name is non-stupid (IE has no slashes)
        clear the bindings
        get all the bindings from the corresponding (and clone them?) and add them to the template, if it exists
        also stick the channel into the lookup using a key with a slash for category
the template is now input map
make an empty dict for storing the channel values, could also populate it with default values at this time
```

input getting and storing
```
have lookup dict
have event
for each channel in the lookup dict:
    for each binding in the channel:
        poke it and see if it made a value
        if it did then stick that into the value dict using the key from the lookup dict
        
    if there's still nothing in channel values then stick the default from the channel in
```

## UI
The UI could stay the same but I think we might as well change it now since there are some changes that need to be made:

The accordion menus should not expand onto a single channel, they should expand onto a table of related channels.
The table would just say channel name and then some short stuff (icons and letters made into buttons) that you can click on to edit that thing.
Basically just like beamng.

let's break the stuff first then see how much fixing is needed, if the ui can be restored easily then might as well fix then merge and redo later