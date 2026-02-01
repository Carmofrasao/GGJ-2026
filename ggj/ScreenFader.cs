using Godot;

public partial class ScreenFader : CanvasLayer
{
	[Export] public ColorRect FadeRect;
	[Export] public bool AutoFadeInOnReady = true;
	[Export] public float AutoFadeInDuration = 0.5f;

	public override void _Ready()
	{
		// Force full black at scene start
		var c = FadeRect.Color;
		c.R = 0; c.G = 0; c.B = 0;
		c.A = 1.0f;
		FadeRect.Color = c;

		if (AutoFadeInOnReady)
			CallDeferred(nameof(StartAutoFadeIn)); // important
	}

	private void StartAutoFadeIn()
	{
		FadeFromBlackToTransparent(AutoFadeInDuration);
	}

	public Tween FadeToBlack(float duration = 0.5f)
	{
		var tween = CreateTween();
		tween.SetPauseMode(Tween.TweenPauseMode.Process); // keeps working even if paused
		tween.TweenProperty(FadeRect, "color:a", 1.0f, duration).FromCurrent();
		return tween;
	}

	public Tween FadeFromBlackToTransparent(float duration = 0.5f)
	{
		var c = FadeRect.Color;
		c.A = 1.0f;
		FadeRect.Color = c;

		var tween = CreateTween();
		tween.SetPauseMode(Tween.TweenPauseMode.Process);
		tween.TweenProperty(FadeRect, "color:a", 0.0f, duration);
		return tween;
	}

	// Alias (so older calls still work)
	public Tween FadeFromBlack(float duration = 0.5f) => FadeFromBlackToTransparent(duration);
}
