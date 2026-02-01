using Godot;
using System;

public partial class ZoomInTest : Area2D
{
	[Export] public Camera camera;
	[Export] public Character character { get; set; }
	[Export] public Gate gate;
	
	private Vector2 cam5 = new Vector2(2432,  1088); // Cam5
	private Vector2 cam4 = new Vector2(2406,  540); // Cam4
	
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
		
		camera.GlobalPosition = cam5;
		camera.zoomIn = true;
		camera.setZoom(new Vector2(5f, 5f));
		
	}
	
	private void OnBodyExited(Node2D body)
	{
		if (body is not Character) return;
		
		camera.GlobalPosition = cam4;
		camera.zoomIn = false;
		camera.setZoom(new Vector2(1f, 1f));
		
		GetParent().GetNode<AudioStreamPlayer>("MusicPlayer").Stop();
		GetParent().GetNode<AudioStreamPlayer>("BossPlayer").Play();
		
		gate.setGate();
	}
}
