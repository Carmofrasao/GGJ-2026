using Godot;
using System;

public partial class RoomLeaveTrigger : Area2D
{
	[Export] public Vector2 HorizontalPosition { get; set; }
	[Export] public Character character { get; set; }
	[Export] public float PushAmount { get; set; }
	[Export] public Camera2D GameCamera { get; set; }
	
	private float lastDir = 0.0f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Character character) // your player class
			return;
		
		float dir = Mathf.Sign(character.Velocity.X);
		
		GD.Print(dir);

		// SNAP
		if (lastDir != dir)
		{
			GameCamera.GlobalPosition += new Vector2(HorizontalPosition.X * dir, HorizontalPosition.Y);
			character.GlobalPosition += new Vector2(PushAmount * dir, 0);
			lastDir = dir;
		}

		// Optional: if you use camera smoothing, turn it off or it won't feel like a snap
		GameCamera.PositionSmoothingEnabled = false;
	}
}
