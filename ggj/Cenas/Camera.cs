using Godot;
using System;

// Cam1 -972 -540
// Cam2 942 -540
// Cam3 486 540
// Cam4 2406 540

public partial class Camera : Camera2D
{
	[Export] public Character character;
	
	public bool zoomIn {get; set;}

	private static readonly Vector2[] CamTopLefts =
	{
		new Vector2(-972, -540), // Cam1
		new Vector2( 942, -540), // Cam2
		new Vector2( 486,  540), // Cam3
		new Vector2(2406,  540), // Cam4
	};

	public void CameraPanning()
	{
		if (character == null) return;

		Vector2 player = character.GlobalPosition;             // player center
		Vector2 viewSize = GetViewportRect().Size;             // camera view width/height

		// If player is still inside the CURRENT camera view rect, do nothing
		Rect2 currentRect = new Rect2(GlobalPosition, viewSize); // GlobalPosition treated as top-left
		if (currentRect.HasPoint(player))
			return;

		// Otherwise, switch to the camera whose rect contains the player
		for (int i = 0; i < CamTopLefts.Length; i++)
		{
			Rect2 camRect = new Rect2(CamTopLefts[i], viewSize);
			if (camRect.HasPoint(player))
			{
				GlobalPosition = CamTopLefts[i];
				return;
			}
		}

		// Optional fallback: if player isn't inside any rect, snap to nearest
		Vector2 best = CamTopLefts[0];
		float bestD2 = (player - CamTopLefts[0]).LengthSquared();

		for (int i = 1; i < CamTopLefts.Length; i++)
		{
			float d2 = (player - CamTopLefts[i]).LengthSquared();
			if (d2 < bestD2) { bestD2 = d2; best = CamTopLefts[i]; }
		}

		GlobalPosition = best;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CameraPanning();
	}
}
