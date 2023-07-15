@tool
extends EditorPlugin

func _enter_tree():
	add_custom_type("FontAwesome", "Label",preload("FontAwesome.gd"), null )
	add_custom_type("FontAwesomeButton", "Button", preload("FontAwesomeButton.gd"), null)

func _exit_tree():
	remove_custom_type("FontAwesome")