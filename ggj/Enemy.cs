using Godot;
using System;
using static Godot.GD;

public partial class Enemy : CharacterBody2D
{
	private float Speed = 100;
	private bool IsChasing = false;
	private Node2D Player = null;
	private float Gravity = 500;
	
	private void OnArea2DBodyEntered(Node2D body)
	{
		GD.Print("Algo entrou: ", body.Name);
		if (body is Node2D player && body.IsInGroup("Player"))
		{
			Player = player;
			IsChasing = true;
			GD.Print("Player entrou na área");
		}
	}
	
	private void OnArea2DBodyExited(Node2D body)
	{
		Print("saiu");
		Player = null;
		IsChasing = false;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (IsChasing && Player != null) {
			var dir = Player.GlobalPosition.X - GlobalPosition.X;
			Velocity = new Vector2(Mathf.Sign(dir) * Speed, Velocity.Y);
		} else {
			Velocity = new Vector2(Mathf.Lerp(Velocity.X, 0.0f, 0.1f), Velocity.Y);
		}
		
		MoveAndCollide(Velocity); 	
	}
}
