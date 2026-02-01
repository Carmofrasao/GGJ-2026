using Godot;
using System;

public partial class Damageable : Node2D
{
	[Signal]
	public delegate void SetHealthEventHandler(float  CurrentHealth);
	
	[Export] public float Health;
	[Export] public AnimatedSprite2D ShadedSprite;
	
	public override void _Process(double delta) {
		var timer = GetNode<Timer>("FlashTimer");
		var percent = timer.GetTimeLeft() / timer.GetWaitTime();
		((ShaderMaterial)ShadedSprite.GetMaterial()).SetShaderParameter("flash_percent", percent);
	}
	
	public bool TakeDamage(float damageAmount) {
		if (!GetNode<Timer>("InvulTimer").IsStopped()) {
			return false;
		}
		GetNode<Timer>("InvulTimer").Start();
		Health = Mathf.Max(0.0f, Health - damageAmount);
		EmitSignal(nameof(SetHealth), Health);
		if (Health == 0) {
			// DIE!
			GD.Print("Died!");
			if (GetParent() is Character)
			{
				GetTree().ChangeSceneToFile("res://MainMenu.tscn");
			}
			GetParent().QueueFree();
			
		}
		GetNode<Timer>("FlashTimer").Start();
		if (GetParent() is Enemy enemy) {
			enemy.CurrentState = EnemyState.Stunned;
		}
		return true;
	}
	
	public void Attack(Damageable damageable, float damageAmount) {
		if (!GetNode<Timer>("AttackCooldownTimer").IsStopped()) {
			return;
		}
		GetNode<Timer>("AttackCooldownTimer").Start();
		if (damageable.TakeDamage(damageAmount)) {
			if (damageable.GetParent() is Character character) {
				if (GetParent().GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH) {
					character.KnockbackSpeed = -1f;
				} else {
					character.KnockbackSpeed = 1f;
				}
			}
		}
	}
}
