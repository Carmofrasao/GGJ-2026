using Godot;
using System;

public partial class Character : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 10;
	public Vector2 Velocity = new Vector2();
	public float JumpVelocity = -10f;
	public bool IsGround = true;
	public bool IsJumping = false;
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
		Velocity = inputDirection * Speed;
		GD.Print("direction", inputDirection);
		
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetInput();
		GD.Print(Velocity);
		MoveAndCollide(Velocity);
	}
}
