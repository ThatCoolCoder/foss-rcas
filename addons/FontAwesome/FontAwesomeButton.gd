@tool
extends Button

# (this script is not part of the original addon)

var _fa
var _label

@export var font_awesome_icon = "":
    get:
        return font_awesome_icon
    set(value):
        font_awesome_icon = value
        if is_inside_tree():
            _fa.icon = font_awesome_icon

@export var label_text = "":
    get:
        return label_text
    set(value):
        label_text = value
        if is_inside_tree():
            _label.text = text

@export var text_theme_type_variation = "":
    get:
        return text_theme_type_variation
    set(value):
        text_theme_type_variation = value
        if is_inside_tree():
            _fa.theme_type_variation = value
            _label.theme_type_variation = value

func _init():
    icon = icon
    # filter = filter

func _ready():
    # var container = HBoxContainer.new()
    # container.anchors_preset = PRESET_FULL_RECT
    # add_child(container)
    
    # _fa = Label.new()
    # _fa.set_script(preload("res://addons/FontAwesome/FontAwesome.gd"))
    # container.add_child(_fa)

    # _label = Label.new()
    # container.add_child(_label)

    # font_awesome_icon = font_awesome_icon
    # label_text = label_text
    # text_theme_type_variation = text_theme_type_variation

