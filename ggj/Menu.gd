extends CanvasLayer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if Input.is_action_just_pressed("menu"):
		if visible == false:
			get_tree().paused = true;
			visible = true;
		else:
			get_tree().paused = false;
			visible = false;
	pass

func _on_continue_button_down() -> void:
	get_tree().paused = false;
	visible = false;


func _on_exit_game_pressed() -> void:
	get_tree().quit();


func _on_main_menu_pressed() -> void:
	get_tree().paused = false;
	get_tree().change_scene_to_file("res://MainMenu.tscn");
