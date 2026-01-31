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
	private float Speed = 500;
	private float Gravity = 500;
	private Node2D Player;
	private EnemyState CurrentState = EnemyState.Idle;
	
	public override void _PhysicsProcess(double delta)
	{
		Print("CurrentState = " + EnemyState.ChasingPlayer + " Player = " + Player);
		if (CurrentState == EnemyState.ChasingPlayer && Player != null) {
			var dir = Player.GlobalPosition.X - GlobalPosition.X;
			Velocity = new Vector2(Mathf.Sign(dir) * Speed, Velocity.Y);
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
				foreach (var node in GetNode<Area2D>("AttackRange").GetOverlappingBodies()) {
					if (node is Node2D player) {
						// player.RECEIVEDAMAGE();
						CurrentState = EnemyState.OnAttackCooldown;
						GetNode<Timer>("CooldownTimer").Start();
						break;
					}
				}
				foreach (var node in GetNode<Area2D>("AwareArea").GetOverlappingBodies()) {
					if (node is Node2D player) {
						Player = player;
						CurrentState = EnemyState.ChasingPlayer;
						break;
					}
				}
				CurrentState = EnemyState.Idle;
				break;
			case EnemyState.OnAttackCooldown:
				if (GetNode<Timer>("CooldownTimer").IsStopped())
					CurrentState = EnemyState.Idle;
				break;
			case EnemyState.Stunned:
				if (GetNode<Timer>("StunTimer").IsStopped())
					CurrentState = EnemyState.Idle;
				break;
		}
	}
}
