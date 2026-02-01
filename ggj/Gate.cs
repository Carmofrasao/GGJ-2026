using Godot;
using System;

public partial class Gate : StaticBody2D
{
	[Export] private Sprite2D sprite;
	[Export] private CollisionShape2D collision;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite.Visible = false;
		collision.SetDeferred("disabled", true);
		//collision.SetDisabled(true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void setGate(){
		sprite.Visible = true;
		//collision.CallDeferred("set_disable", false);
		collision.SetDeferred("disabled", false);
		
	}
}
