using Godot;
using System;

public partial class Damageable : Node2D
{
	[Export] public float InitialHealth;
	private float Health;
	
	public override void _Ready() {
		Health = InitialHealth;
	}
	
	public void TakeDamage(float damageAmount) {
		if (!GetNode<Timer>("InvulTimer").IsStopped()) {
			return;
		}
		GD.Print("Take Damage: " + damageAmount);
		GetNode<Timer>("InvulTimer").Start();
		Health = Mathf.Max(0.0f, Health - damageAmount);
		if (Health == 0) {
			// DIE!
			GD.Print("Died!");
			GetParent().QueueFree();
		}
	}
	
	public void Attack(Damageable damageable, float damageAmount) {
		if (!GetNode<Timer>("AttackCooldownTimer").IsStopped()) {
			return;
		}
		GD.Print("Attacking!");
		GetNode<Timer>("AttackCooldownTimer").Start();
		damageable.TakeDamage(damageAmount);
	}
}
