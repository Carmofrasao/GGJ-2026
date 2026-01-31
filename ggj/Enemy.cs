using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	private float Speed = 100;
	private bool IsChasing = false;
	private Node2D Player = null;
	
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
		
	}
}
