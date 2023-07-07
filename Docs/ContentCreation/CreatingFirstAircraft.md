# Creating your first aircraft for FOSS-RCAS

todo: write this properly, currently it's just notes

## File setup

- create dir 
- create very basic toml, create tscn with rb

## Basic model

- normally the model would be made in a 3d modelling program then imported, but when creating your first aircraft it's best to skip this complication
- make with csg, take care of size

## Basic physics

- rb mass, collisionshapes
- Test it, watch it be thrown and fall to the ground.

## Aero physics

- add simplethruster, set the thrust to one
    - introduce forcers in general, tell them which way is forward
- make wing
    - mirror node
    - don't bother teaching about aerocurves, just tell them to slap on some defaults
    - say which way the arrow needs to point
- make tailplane (but static)

If you press play now and load up your aircraft, it will probably go forwards a bit further now that the motor is pulling and it might even fly a bit. But like this it is just going off on its own. To make it flyable, you need to add control. 


## Control

- introduce controlhub, add a sportcontrolhub
- swap simplethruster for controlledsimplethruster
- introduce controlsurface and add them
    - 4 channel controls
    - remember to reverse one aileron
- try flying, if the controls go in the wrong direction reverse them. now is a handy time to say that you don't need to restart game to refresh, just reload location (and if we get it working you can just ctrl+r).


If you've managed to get this far, congratulations! You're probably ready to move on to creating something more advanced.