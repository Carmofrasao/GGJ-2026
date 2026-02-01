using Godot;
using System;

public partial class MascaraVermelha : Node2D
{	
	public void OnBodyEnteredArea(Node2D node) {
		if (node is Character player) {
			player.UsingRedThing = true;
		}
		SetVisible(false);
	}
}
