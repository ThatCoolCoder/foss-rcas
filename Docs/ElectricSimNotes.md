# Notes on the upcoming electrical simulation:

## Values

Inputs:
- prop: radius, pitch, entry speed, num blades
- motor: no-load resistance
- battery: voltage, max current, internal resistance

Outputs:
- motor: torque applied, current drawn
- prop: thrust generated

## Thinking + equations

Battery controls voltage
Control system controls duty cycle, essentially voltage at motor
Prop + motor controls current
    Current = prop current factor * motor efficiency

Moving magnets <-> current

Need to account for heat losses from resistance
```
mechanical power    = electrical power - heat losses
                    = electrical power - r * i^2
                    = torque * rpm
```

Could find torque from an estimated current
```
torque = (1/kv) * i
```

```
torque  =~= target rpm - rpm ?
        =   (kv * v) - rpm
```

Or could find torque first and then figure out what current would have been 
```
voltage sag = current * internal resistance
```

```
thrust = 1/2 * density * area * (exit speed^2 - entry speed^2)
```

```
torque  = (delta s / max rpm) * peak torque ?
        = ((kv * v - rpm) / (kv * v)) * peak torque
```

```
exit speed = pitch speed * some efficiency coefficient
```

```
t   = kt * i
    = (1/kv) * i

    (kv needs to be in SI)
```

t =~= i =~= load
rpm =~= vf
vf = throttle * v
how to calculate t or i?

Possible chain of calculation:

m prop resistance, prop mass, current prop rpm -> prop rpm
y kv, duty cycle, voltage -> target rpm
y prop rpm, target rpm -> delta s
m delta s, torque factor -> torque
torque, prop mass, current prop rpm -> new prop rpm
torque, torque constant -> current

This seems far too risky, there are so many inaccuracies that will creep in. Models are not created by users, I think it's best to (at least for v1) just have measured or externally computed values, instead of complete simulation.

So perhaps we add the following values to the brushless motor. They are related to the propeller-motor combo so there's no properly neat place.
- amps drawn at full throttle on the ground
- rpm reached at full throttle on the ground (can obtain from audio recording - `= dominant frequency / num prop blades * 60)`)

Then it becomes:
static current, torque constant -> torque
power, maths -> prop resistance
    `drag factor = power/v^2`
prop resistance, prop mass, current prop rpm -> prop rpm

## Resources
making propeller thrust calcs practical: https://www.electricrcaircraftguy.com/2014/04/propeller-static-dynamic-thrust-equation-background.html
a bunch of stuff about efficiency: https://www.tytorobotics.com/blogs/articles/brushless-motor-power-and-efficiency-analysis