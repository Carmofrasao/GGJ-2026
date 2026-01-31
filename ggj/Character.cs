using Godot;
using System;

public partial class Character : CharacterBody2D
{
	[Export] public int Speed { get; set; } = 200;
	
	
	[Export] public float JumpHeight = 110.0f;
	[Export] public float JumpTimetoPeak = 0.5f;
	[Export] public float JumpTimeToDescent = 0.35f;
	
	[Export] public float DoubleJumpHeightMultiplier = 1.1f;
	[Export] public float DoubleJumpFallTimeMultiplier = 0.9f;
	
	[Export] public float GlideTimeToDescent = 3.0f;
	
	 public bool CanDoubleJump;
	 public bool doubleJumpActive = false;
	 public bool glideActive = false;
	 public float JumpVelocity;
	 public float JumpGravity;
	 public float FallGravity;
	
	 // Minha maluquice
	 public float DoubleJumpVelocity;
	 public float DoubleJumpGravity;
	 public float DoubleFallGravity;
	
	// Minha maluquice glide
	public float GlideFallGravity;
	public float DoubleGlideFallGravity;
	
	public override void _Ready()
	{
		JumpVelocity = ((2.0f * JumpHeight) / JumpTimetoPeak) * -1.0f;
		JumpGravity = ((-2.0f * JumpHeight) / (JumpTimetoPeak * JumpTimetoPeak)) * -1.0f;
		FallGravity = ((-2.0f * JumpHeight) / (JumpTimeToDescent * JumpTimeToDescent)) * -1.0f;
		
		// Minha maluquice
		// Double jump: 1.5x height, same time-to-peak => scale both v0 and gravity by height multiplier
		float H2 = JumpHeight * DoubleJumpHeightMultiplier;

		DoubleJumpVelocity = ((2.0f * H2) / JumpTimetoPeak) * -1.0f;
		DoubleJumpGravity  = ((-2.0f * H2) / (JumpTimetoPeak * JumpTimetoPeak)) * -1.0f;

		// Double fall: choose gravity so fall time scales by DoubleJumpFallTimeMultiplier
		// g2 = g * (heightMult / timeMult^2)
		float FallGravityMultiplier = DoubleJumpHeightMultiplier / (DoubleJumpFallTimeMultiplier * DoubleJumpFallTimeMultiplier);
		DoubleFallGravity = FallGravity * FallGravityMultiplier;
		
		// Glide fall gravity (same height, longer descent time)
		GlideFallGravity = ((-2.0f * JumpHeight) / (GlideTimeToDescent * GlideTimeToDescent)) * -1.0f;
		DoubleGlideFallGravity = GlideFallGravity * FallGravityMultiplier;
		
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
		bool rising = Velocity.Y < 0.0f;

		if (rising)
			return doubleJumpActive ? DoubleJumpGravity : JumpGravity;

		// falling:
		if (doubleJumpActive)
			return glideActive ? DoubleGlideFallGravity : DoubleFallGravity;

		return glideActive ? GlideFallGravity : FallGravity;
	}
	
	public void Jump()
	{
		GD.Print("JumpGravity: ", JumpGravity);
		GD.Print("FallGravity: ", FallGravity);
		GD.Print("Jump Velocity: ", JumpVelocity);
		Velocity = new Vector2(Velocity.X, JumpVelocity);
	}
	
	public void DoubleJump()
	{
		if (!CanDoubleJump) return;
		GD.Print("DoubleJump");

		doubleJumpActive = true;
		GD.Print("DoubleJumpGravity: ", DoubleJumpGravity);
		GD.Print("DoubleFallGravity: ", DoubleFallGravity);
		GD.Print("DoubleJumpVelocity: ", DoubleJumpVelocity);
		Velocity = new Vector2(Velocity.X, DoubleJumpVelocity);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		
	}
	
	public override void _PhysicsProcess(double delta)
{
	Velocity = new Vector2(GetInput() * Speed, Velocity.Y);
	
	if (IsOnFloor())
	{
		CanDoubleJump = true;
		doubleJumpActive = false;
		glideActive = false;
	}
	
	if (Input.IsActionJustPressed("up") && IsOnFloor())
	{
		Jump();
	}
	else if (Input.IsActionJustPressed("up") && !IsOnFloor() && CanDoubleJump)
	{
		DoubleJump();
		CanDoubleJump = false;
	}
	
	// Glide
	bool glideHeld = Input.IsActionPressed("glide");
	glideActive = glideHeld && !IsOnFloor() && Velocity.Y > 0.0f;
	
	Velocity = new Vector2(
		Velocity.X,
		Velocity.Y + GetGravity() * (float)delta
	);
	MoveAndSlide();
}

}
