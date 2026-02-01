using Godot;
using System;
using static Godot.GD;

enum EnemyState {
	Idle,
	ChasingPlayer,
	Stunned,
	OnAttackCooldown
};

public partial class Enemy : CharacterBody2D
{
	private float Speed = 200;
	private float Gravity = 500;
	private Node2D Player;
	private EnemyState CurrentState = EnemyState.Idle;
	private float CurrentHorizontalSpeed = 0;
	[Export] public float HorizontalDecaySpeed = 0.09f;
	[Export] public float HorizontalAcceleration = 0.15f;
	
	public float GetHorizontalVelocity(float dir)
	{
		if (dir == 0) {
			if (CurrentHorizontalSpeed > 0)
				CurrentHorizontalSpeed = Mathf.Max(0, CurrentHorizontalSpeed - HorizontalDecaySpeed);
			else
				CurrentHorizontalSpeed = Mathf.Min(0, CurrentHorizontalSpeed + HorizontalDecaySpeed);
			
			return CurrentHorizontalSpeed;
		}
		
		if (dir == -1)
			CurrentHorizontalSpeed -= HorizontalAcceleration;
		if (dir == +1)
			CurrentHorizontalSpeed += HorizontalAcceleration;
			
		CurrentHorizontalSpeed = Mathf.Clamp(CurrentHorizontalSpeed, -2.0f, 2.0f);

		return CurrentHorizontalSpeed;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (CurrentState == EnemyState.ChasingPlayer && Player != null) {
			var dir = Mathf.Sign(Player.GlobalPosition.X - GlobalPosition.X);
			Velocity = new Vector2(GetHorizontalVelocity(dir) * Speed, Velocity.Y);
		} else {
			Velocity = new Vector2(Mathf.Lerp(Velocity.X * 0.8f, 0.0f, 0.1f), Velocity.Y);
		}
		
		Velocity = new Vector2(
			Velocity.X,
			Velocity.Y + 200 * (float)delta
		);
		
		MoveAndSlide();
	}
	
	public void ReceiveDamage() {
		CurrentState = EnemyState.Stunned;
	}
	
	public override void _Process(double delta) 
	{
		switch (CurrentState) {
			case EnemyState.Idle:
				foreach (var node in GetNode<Area2D>("AwareArea").GetOverlappingBodies()) {
					if (node is Node2D player) {
						Player = node;
						CurrentState = EnemyState.ChasingPlayer;
						break;
					}
				}
				break;
			case EnemyState.ChasingPlayer:
				bool ok = false;
				foreach (var node in GetNode<Area2D>("AttackRange").GetOverlappingBodies()) {
					if (node is Node2D player) {
						var damageable = node.GetNode<Damageable>("Damageable");
						if (damageable != null) {
							GetNode<Damageable>("Damageable").Attack(damageable, 1.1f*RandRange(6, 10));
						}
						CurrentState = EnemyState.OnAttackCooldown;
						GetNode<Timer>("AttackCooldownTimer").Start();
						ok = true;
						break;
					}
				}
				if (!ok)
					foreach (var node in GetNode<Area2D>("AwareArea").GetOverlappingBodies()) {
						if (node is Node2D player) {
							Player = player;
							CurrentState = EnemyState.ChasingPlayer;
							ok = true;
							break;
						}
					}
				if (!ok)
					CurrentState = EnemyState.Idle;
				break;
			case EnemyState.OnAttackCooldown:
				if (GetNode<Timer>("AttackCooldownTimer").IsStopped())
					CurrentState = EnemyState.Idle;
				break;
			case EnemyState.Stunned:
				if (GetNode<Timer>("StunTimer").IsStopped())
					CurrentState = EnemyState.Idle;
				break;
		}
	}
}
