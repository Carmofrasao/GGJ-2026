using Godot;
using System;

public partial class Menu : CanvasLayer
{
	[Export] private Button Continue;
	[Export] private Button Quit;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnContinuePressed()
	{
		GD.Print("Apertei");
		GetTree().Paused = false;
		Visible = false;
	}
}
