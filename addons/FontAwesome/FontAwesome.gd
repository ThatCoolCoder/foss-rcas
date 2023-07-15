@tool
extends Label

@export_range(0, 1000, 1) var icon_size: int = 16:
    get:
        return icon_size
    set(value):
        icon_size = value
        if is_inside_tree():
            add_theme_font_size_override("font_size", icon_size)

@export var icon = "":
    get:
        return icon
    set(value):
        icon = value
        if is_inside_tree():
            var iconcode = ""
            if value in Cheatsheet:
                iconcode = Cheatsheet[value]
            set_text(iconcode)

@export var filter = CanvasItem.TEXTURE_FILTER_PARENT_NODE:
    get:
        return filter
    set(value):
        filter = value
        texture_filter = filter

const ttf = preload("fontawesome-webfont.ttf")
const Cheatsheet = preload("Cheatsheet.gd").Cheatsheet

func _init():
    icon_size = icon_size
    icon = icon
    filter = filter
    add_theme_font_override("custom_font", ttf)

func _ready():
    icon_size = icon_size
    icon = icon