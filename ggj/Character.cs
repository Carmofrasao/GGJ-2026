using Godot;
using System;

public partial class Character : CharacterBody2D
{
	[Export] public int Speed { get; set; } = 10;
	
	
	[Export] public float JumpHeight;
	[Export]public float JumpTimetoPeak;
	[Export] public float JumpTimeToDescent;
	
	 public float JumpVelocity;
	 public float JumpGravity;
	 public float FallGravity;
	
	public override void _Ready()
	{
		JumpVelocity = ((2.0f * JumpHeight) / JumpTimetoPeak) * -1.0f;
		JumpGravity = ((-2.0f * JumpHeight) / (JumpTimetoPeak * JumpTimetoPeak)) * -1.0f;
		FallGravity = ((-2.0f * JumpHeight) / (JumpTimeToDescent * JumpTimeToDescent)) * -1.0f;
	}
	
	public float GetInput()
	{
		float horizontal = 0.0f;

		if (Input.IsActionPressed("left"))
			horizontal = -1.0f;

		if (Input.IsActionPressed("right"))
			horizontal = 1.0f;

		return horizontal;
	}

	public float GetGravity()
	{
		return Velocity.Y < 0.0f ? JumpGravity : FallGravity;
	}
	
	public void Jump()
	{
		GD.Print("Jump");
		Velocity = new Vector2(Velocity.X, JumpVelocity);
	}
	


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		
	}
	
	public override void _PhysicsProcess(double delta)
{
	Velocity = new Vector2(GetInput() * Speed, Velocity.Y);
	
	if (Input.IsActionJustPressed("up") && IsOnFloor())
		Jump();
	
	Velocity = new Vector2(
		Velocity.X,
		Velocity.Y + GetGravity() * (float)delta
	);
	MoveAndSlide();
}

}
