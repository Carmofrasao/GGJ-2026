extends CharacterBody2D


var Speed = 100;
var PlayerChase = false;
var Player = null;

func _physics_process(delta: float) -> void:
	if PlayerChase:
		position += (Player.position - position).normalized() * Speed * delta
		
	move_and_collide(Vector2(0,0)) 
	


func _on_area_2d_body_entered(body: Node2D) -> void:
	print("entrou");
	Player = body;
	PlayerChase = true;

func _on_area_2d_body_exited(body: Node2D) -> void:
	print("saiu");
	Player = null;
	PlayerChase = false;
