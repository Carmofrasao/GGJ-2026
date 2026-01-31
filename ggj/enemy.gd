extends CharacterBody2D


var Speed = 100;
var Gravity = 500;
var PlayerChase = false;
var Player = null;
var Spawn;


func _physics_process(delta: float) -> void:
	if PlayerChase and Player:
		var dir = Player.global_position.x - global_position.x
		velocity.x = sign(dir) * Speed
	else:
		velocity.x = lerp(velocity.x, 0.0, 0.1)
	
	move_and_collide(velocity) 
	





func _on_area_2d_body_entered(body: Node2D) -> void:
	print("entrou");
	Player = body;
	PlayerChase = true;

func _on_area_2d_body_exited(body: Node2D) -> void:
	print("saiu");
	Player = null;
	PlayerChase = false;


func _on_ready() -> void:
	Spawn = position;
	pass # Replace with function body.
