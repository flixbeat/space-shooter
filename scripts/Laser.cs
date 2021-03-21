using Godot;
using System;

public class Laser : Sprite
{
    
    [Export] public int damage = 1;
    [Export] private float speed = 500;

    private Area2D area;

    public override void _Ready()
    {
        area = GetNode<Area2D>("Area2D");
        area.Connect("area_entered", this, nameof(OnAreaEntered));
    }

    public override void _Process(float delta)
    {
        Translate(Vector2.Up * delta * speed);

        if(Position.y < -100)
            QueueFree();
    }

    private void OnAreaEntered(Area2D other)
    {
        QueueFree();
    }
}
