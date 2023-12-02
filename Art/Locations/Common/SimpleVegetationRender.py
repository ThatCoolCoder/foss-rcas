import math
import time
import bpy
import os

# Render the item at a bunch of angles (configurable) and save it
# Usage: Parent your camera to an object called CameraHolder that is located in the center of your vegetation

# Todo: bake normals for each angle too

num_steps = 3
descriptor = ''

holder = bpy.data.objects['CameraHolder']

filepath = bpy.data.filepath
(directory, file) = os.path.split(filepath)
name = os.path.splitext(file)[0]

for i in range(num_steps):
    angle = math.pi * 2 / num_steps * i
    holder.rotation_euler[2] = angle
    angle_int = int(round(math.degrees(angle)))
    bpy.context.scene.render.filepath = os.path.join(directory, f'{name}{descriptor}{angle_int}.png')
    bpy.ops.render.render(write_still = True)


holder.rotation_euler[2] = math.radians(0)
