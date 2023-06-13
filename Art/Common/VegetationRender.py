import math
import time
import bpy
import os

# Render the item at a bunch of angles (configurable) and save it, also saves normals
# Usage: Parent your camera to an object called CameraHolder that is located in the center of your vegetation.
# Auto-sets up compositor, etc
# Is somewhat inefficient in that we have to render twice to get diffuse and normal

num_steps = 3

tree = bpy.context.scene.node_tree
links = tree.links

def prepare_render_settings():
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.view_layers['ViewLayer'].use_pass_normal = True
    bpy.context.scene.view_layers['ViewLayer'].use_pass_diffuse_color = True
    
    bpy.context.scene.use_nodes = True

    for n in tree.nodes:
        tree.nodes.remove(n)

    rl = tree.nodes.new('CompositorNodeRLayers')
    
    output = tree.nodes.new(type='CompositorNodeComposite')

    # Fix cycles normals being non-standard
    multiply = tree.nodes.new(type='CompositorNodeMixRGB')
    multiply.blend_type = 'MULTIPLY'
    multiply.inputs[2].default_value = (0.5, 0.5, 0.5, 1)

    add = tree.nodes.new(type='CompositorNodeMixRGB')
    add.blend_type = 'ADD'
    add.inputs[2].default_value = (0.5, 0.5, 0.5, 1)

    invert = tree.nodes.new(type='CompositorNodeInvert')
    links.new(rl.outputs[2], multiply.inputs[1])
    links.new(multiply.outputs[0], add.inputs[1])
    links.new(add.outputs[0], invert.inputs[1])

    links.new(rl.outputs[1], output.inputs[1]) # link alpha 

    return rl.outputs[3], invert.outputs[0], output.inputs[0]

def prepare_for_render(image, normal, output, render_normal=False):
    if render_normal:
        links.new(normal, output)
    else:
        links.new(image, output)

holder = bpy.data.objects['CameraHolder']

filepath = bpy.data.filepath
(directory, file) = os.path.split(filepath)
name = os.path.splitext(file)[0]

#image, normal, output = prepare_render_settings()

default_engine = bpy.context.scene.render.engine

for i in range(num_steps):
    angle = math.pi * 2 / num_steps * i
    holder.rotation_euler[2] = angle
    angle_int = int(round(math.degrees(angle)))

    bpy.context.scene.view_layers["ViewLayer"].material_override = None
    bpy.context.scene.render.filepath = os.path.join(directory, f'Render/{name}{angle_int}.png')
    bpy.ops.render.render(write_still=True)

    bpy.context.scene.render.engine = 'CYCLES'
    bpy.context.scene.view_layers["ViewLayer"].material_override = bpy.data.materials["Normal"]
    bpy.context.scene.render.filepath = os.path.join(directory, f'Render/{name}{angle_int}Norm.png')
    bpy.ops.render.render(write_still=True)
    bpy.context.scene.render.engine = default_engine



holder.rotation_euler[2] = math.radians(0)

