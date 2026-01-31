using Godot;
using System;

public partial class Damageable : Node2D
{
	[Export] public float InitialHealth;
	private float Health;
	[Export] public AnimatedSprite2D ShadedSprite;
	
	public override void _Ready() {
		Health = InitialHealth;
	}
	
	public override void _Process(double delta) {
		var timer = GetNode<Timer>("FlashTimer");
		var percent = timer.GetTimeLeft() / timer.GetWaitTime();
		((ShaderMaterial)ShadedSprite.GetMaterial()).SetShaderParameter("flash_percent", percent);
	}
	
	public void TakeDamage(float damageAmount) {
		if (!GetNode<Timer>("InvulTimer").IsStopped()) {
			return;
		}
		GD.Print("Take Damage: " + damageAmount + " Have Health: " + Health);
		GetNode<Timer>("InvulTimer").Start();
		Health = Mathf.Max(0.0f, Health - damageAmount);
		if (Health == 0) {
			// DIE!
			GD.Print("Died!");
			GetParent().QueueFree();
		}
		GetNode<Timer>("FlashTimer").Start();
	}
	
	public void Attack(Damageable damageable, float damageAmount) {
		if (!GetNode<Timer>("AttackCooldownTimer").IsStopped()) {
			GD.Print("Trying to attack " + damageable + " but not working using timer " + GetNode<Timer>("AttackCooldownTimer") + "!");
			return;
		}
		GD.Print("Attacking with timer " + GetNode<Timer>("AttackCooldownTimer") + "!");
		GetNode<Timer>("AttackCooldownTimer").Start();
		damageable.TakeDamage(damageAmount);
	}
}
