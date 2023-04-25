# export a control surface at the origin instead of keeping its original offset

import bpy
import os

delta = bpy.context.active_object.location.copy()

for obj in bpy.context.selected_objects:
    obj.location -= delta

filepath = bpy.data.filepath
(directory, file) = os.path.split(filepath)
name = os.path.splitext(file)[0]
save_path = os.path.join(directory, bpy.context.active_object.name + '.gltf')

bpy.ops.export_scene.gltf(filepath=save_path, use_active_collection=True, export_apply=True)

for obj in bpy.context.selected_objects:
    obj.location += delta