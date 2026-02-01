using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Export] private TextureRect Mask;
	[Export] private TextureProgressBar LifeBar;
	[Export] private Character Player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var damageable = Player.GetNode<Damageable>("Damageable");
		if(damageable != null)
			damageable.SetHealth += SetHealth;
	}
	
	public void SetHealth(float Health)
	{
		GD.Print("atualizei a vida");
		LifeBar.Value = Health;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
