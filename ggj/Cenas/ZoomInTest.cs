using Godot;
using System;

public partial class ZoomInTest : Area2D
{
	[Export] public Camera2D camera;
	[Export] public Character character { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (body is not Character) return;
	}
	
	private void OnBodyExited(Node2D body)
	{
		if (body is not Character) return;
	}
}
