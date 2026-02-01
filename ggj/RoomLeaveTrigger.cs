using Godot;
using System.Threading.Tasks;

public partial class RoomLeaveTrigger : Area2D
{	
	[Export] public ScreenFader Fader { get; set; }
	[Export] public Node2D RoomToReplace { get; set; }
	[Export] public PackedScene TargetRoomScene { get; set; }
	
	[Export] public Character character { get; set; }
	[Export] public float HorizontalCell { get; set; }
	[Export] public float VerticalCell { get; set; }
	[Export] public float CellSize { get; set; } = 54.0f;
	
	// CHaracter character

	// Optional: prevents double-triggering if the player overlaps for multiple frames
	private bool _loading = false;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private async Task DoRoomLeave()
	{
		// Fade out
		await ToSignal(Fader.FadeToBlack(0.5f), Tween.SignalName.Finished);

		if (RoomToReplace == null || TargetRoomScene == null)
		{
			GD.PushError("RoomLeaveTrigger: RoomToReplace or TargetRoomScene is null.");
			_loading = false;
			return;
		}

		// Save placement info
		var parent = RoomToReplace.GetParent();
		int index = RoomToReplace.GetIndex();
		var oldTransform = RoomToReplace.GlobalTransform;

		// Wait a frame so the old room is actually freed (prevents duplicates / overlap issues)
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		// Add new room
		var newRoom = TargetRoomScene.Instantiate<Node2D>();
		parent.AddChild(newRoom);
		parent.MoveChild(newRoom, index);

		// Put it exactly where the old one was
		newRoom.GlobalTransform = oldTransform;

		// Remove old room
		RoomToReplace.QueueFree();

		// Update reference so next trigger works if needed
		RoomToReplace = newRoom;
		
		GD.Print(character.GlobalPosition);
		
		if (VerticalCell != 0.0f && HorizontalCell != 0.0f)
			character.GlobalPosition = new Vector2(character.GlobalPosition.X + (HorizontalCell * CellSize), character.GlobalPosition.Y + (VerticalCell * CellSize));
		else if (VerticalCell != 0.0f)
			character.GlobalPosition = new Vector2(character.GlobalPosition.X, character.GlobalPosition.Y  + (VerticalCell * CellSize));
		else if (HorizontalCell != 0.0f)
			character.GlobalPosition = new Vector2(character.GlobalPosition.X + (HorizontalCell * CellSize), character.GlobalPosition.Y);

		GD.Print(character.GlobalPosition);

		// Fade in
		await ToSignal(Fader.FadeFromBlack(0.5f), Tween.SignalName.Finished);

		_loading = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (_loading) return;
		if (body is not Character) return;
		if (Fader == null) return;
		if (RoomToReplace == null) return;
		if (TargetRoomScene == null) return;

		GD.Print("Entrou");

		_loading = true;
		_ = DoRoomLeave();
	}
}
