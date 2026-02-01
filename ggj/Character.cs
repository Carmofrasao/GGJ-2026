using Godot;
using System;
using static Godot.GD;

public partial class Character : CharacterBody2D
{

	[Export] public int Speed { get; set; } = 200;
	
	[Export] public float CurrentHorizontalSpeed = 0;
	
	[Export] public float HorizontalDecaySpeed = 0.09f;
	[Export] public float HorizontalAcceleration = 0.15f;
	
	[Export] public float JumpHeight = 200.0f;
	[Export] public float JumpTimetoPeak = 0.4f;
	[Export] public float JumpTimeToDescent = 0.3f;

	[Export] public float DoubleJumpHeightMultiplier = 1.5f;
	
	[Export] public float GlideTimeToDescent = 0.9f;
	
	 public float KnockbackSpeed = 0.0f;
	
	 public bool CanDoubleJump;
	 public bool doubleJumpActive = false;
	 public bool glideActive = false;
	 public float JumpVelocity;
	 public float JumpGravity;
	 public float FallGravity;
	 public bool IsAttacking = false;
	
	 public bool UsingRedThing = false;
	
	 // Minha maluquice
	 public float DoubleJumpVelocity;
	
	public override void _Ready()
	{
		JumpVelocity = ((2.0f * JumpHeight) / JumpTimetoPeak) * -1.0f;
		JumpGravity = ((-2.0f * JumpHeight) / (JumpTimetoPeak * JumpTimetoPeak)) * -1.0f;
		FallGravity = ((-2.0f * JumpHeight) / (JumpTimeToDescent * JumpTimeToDescent)) * -1.0f;
		
		// Minha maluquice
		// Double jump: 1.5x height, same time-to-peak => scale both v0 and gravity by height multiplier
		float H2 = JumpHeight * DoubleJumpHeightMultiplier;

		DoubleJumpVelocity = (H2 / JumpTimetoPeak) * -1.0f;
	}
	
	public float GetHorizontalVelocity()
	{
		CurrentHorizontalSpeed += KnockbackSpeed;
		if (KnockbackSpeed > 0)
			KnockbackSpeed = Mathf.Max(0, KnockbackSpeed - 3*HorizontalDecaySpeed);
		else if (KnockbackSpeed < 0)
			KnockbackSpeed = Mathf.Min(0, KnockbackSpeed + 3*HorizontalDecaySpeed);
		
		if (!Input.IsActionPressed("left") && !Input.IsActionPressed("right")) {
			if (CurrentHorizontalSpeed > 0)
				CurrentHorizontalSpeed = Mathf.Max(0, CurrentHorizontalSpeed - HorizontalDecaySpeed);
			else
				CurrentHorizontalSpeed = Mathf.Min(0, CurrentHorizontalSpeed + HorizontalDecaySpeed);
			
			return CurrentHorizontalSpeed;
		}
		
		if (Input.IsActionPressed("left"))
			CurrentHorizontalSpeed -= HorizontalAcceleration;
		if (Input.IsActionPressed("right"))
			CurrentHorizontalSpeed += HorizontalAcceleration;
			
		CurrentHorizontalSpeed = Mathf.Clamp(CurrentHorizontalSpeed, -2.0f, 2.0f);

		return CurrentHorizontalSpeed;
	}
	
	public void AttackInFront() {
		GetNode<AudioStreamPlayer>("PunchPlayer").Play();
		GetNode<Damageable>("Damageable").GetNode<Timer>("AttackCooldownTimer").Stop();
		foreach (var node in GetNode<Area2D>("AttackHitbox").GetOverlappingBodies()) {
			var damageable = node.GetNode<Damageable>("Damageable");
			if (damageable != null) {
				GetNode<Damageable>("Damageable").Attack(damageable, 1);
			}
		}
	}

	public float GetGravity()
	{
		bool rising = Velocity.Y < 0.0f;
		if (rising)
			return JumpGravity;
		return FallGravity;
	}
	
	public void Jump()
	{
		Velocity = new Vector2(Velocity.X, JumpVelocity);
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("jump_up");
	}
	
	public void DoubleJump()
	{
		if (!CanDoubleJump) return;

		doubleJumpActive = true;
		Velocity = new Vector2(Velocity.X, DoubleJumpVelocity);
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("jump_up");
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Frame = 0;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsOnFloor() && !IsAttacking)
			if (!Input.IsActionPressed("left") && !Input.IsActionPressed("right")) {
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");
			} else {
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("run");
			}
		
		if (Input.IsActionPressed("left")) {
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
			GetNode<Area2D>("AttackHitbox").SetScale(new Vector2(-1.0f, 1.0f));
		}
		if (Input.IsActionPressed("right")) {
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
			GetNode<Area2D>("AttackHitbox").SetScale(new Vector2(1.0f, 1.0f));
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		float HorizontalVelocity = GetHorizontalVelocity() * Speed;
		
		if (IsOnFloor())
		{
			CanDoubleJump = true;
			doubleJumpActive = false;
			glideActive = false;
			HorizontalVelocity *= 0.9f;
		}
		
		Velocity = new Vector2(HorizontalVelocity, Velocity.Y);
		
		if (Input.IsActionJustPressed("up") && IsOnFloor())
		{
			Jump();
		}
		else if (Input.IsActionJustPressed("up") && !IsOnFloor() && CanDoubleJump)
		{
			DoubleJump();
			CanDoubleJump = false;
		}
		
		if (Velocity.Y > 0.0f && !IsOnFloor()) {
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("jump_down");
		}
		
		// Glide
		bool glideHeld = Input.IsActionPressed("glide");
		glideActive = glideHeld && !IsOnFloor() && Velocity.Y > 0.0f;
		
		Velocity = new Vector2(
			Velocity.X,
			Velocity.Y + GetGravity() * (float)delta
		);
		
		Velocity = new Vector2(
			Velocity.X,
			Velocity.Y - 300*Mathf.Abs(KnockbackSpeed)
		);
		
		if (glideActive)
			Velocity = new Vector2(
				Velocity.X,
				Mathf.Min(Velocity.Y, GlideTimeToDescent*Speed)
			);
		
		if (Input.IsActionJustPressed("attack")) {
			if (GetNode<Damageable>("Damageable").GetNode<Timer>("AttackCooldownTimer").IsStopped()) {
				IsAttacking = true;
				if (UsingRedThing) {
					GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("angry_punch");
					GetNode<Timer>("IreTimer1").Start();
					GetNode<Timer>("IreTimer2").Start();
					GetNode<Timer>("IreTimer3").Start();
					GetNode<Timer>("IreTimer4").Start();
					GetNode<Timer>("IreTimer5").Start();
				} else
					GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("punch");
				GetNode<AudioStreamPlayer>("PunchPlayer").Play();
			}
			foreach (var node in GetNode<Area2D>("AttackHitbox").GetOverlappingBodies()) {
				var damageable = node.GetNode<Damageable>("Damageable");
				if (damageable != null) {
					GetNode<Damageable>("Damageable").Attack(damageable, 1);
				}
			}
			if (GetNode<Damageable>("Damageable").GetNode<Timer>("AttackCooldownTimer").IsStopped())
				GetNode<Damageable>("Damageable").GetNode<Timer>("AttackCooldownTimer").Start();
		}
		
		if (IsAttacking && (
			!(GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "punch" ||
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "angry_punch")
			|| !GetNode<AnimatedSprite2D>("AnimatedSprite2D").IsPlaying()
		)) {
			IsAttacking = false;
		}
		
		MoveAndSlide();
	}

}
