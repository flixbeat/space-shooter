using Godot;
using System;

public class EnemyBullet : Sprite
{
    private float speed = 250f;
    private Area2D area;
    public override void _Ready()
    {
        area = GetNode<Area2D>("Area2D");
        area.Connect("area_entered", this, nameof(OnAreaEntered));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Translate(Vector2.Down * delta * speed);

        if(Position.y > 900)
            QueueFree();
    }

    private void OnAreaEntered(Area2D other)
    {
        QueueFree();
    }
}
