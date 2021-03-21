using Godot;
using System;

public class MeteorPatricle : Sprite
{
    private Tween tween;
    private Timer timerDestroy;

    public override void _Ready()
    {
        tween = GetNode<Tween>("Tween");
        timerDestroy = GetNode<Timer>("TimerDestroy");
        timerDestroy.Connect("timeout", this, nameof(RemoveFromScene));
        Spawn();
    }

    private void Spawn()
    {
        Random rand = new Random();
        float x = Utils.RandomRange(-300f,300f);
        float y = Utils.RandomRange(-300f,300f);
        float size = Utils.RandomRange(0.3f, 1.5f);

        Vector2 scale = new Vector2(size, size);
        Vector2 from = Position;
        Vector2 to = new Vector2(from.x + (float)x, from.y + (float)y);

        ApplyScale(scale);

        tween.InterpolateProperty(this, "position", from, to, 5, Tween.TransitionType.Linear,
            Tween.EaseType.InOut);
        tween.Start();

        timerDestroy.WaitTime = Utils.RandomRange(0.5f, 3);
        timerDestroy.Start();
    }

    private void RemoveFromScene()
    {
        QueueFree();
    }
}
