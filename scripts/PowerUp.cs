using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PowerUp : Sprite
{
    
    [Signal] delegate void AttackUpAcquired();
    [Signal] delegate void FireRateUpAcquired();
    [Signal] delegate void SpeedUpAcquired();

    [Export(PropertyHint.Enum, "AttackUp,FireRateUp,SpeedUp")]
    private string type; 
    private const float speed = 100f;
    private Area2D area;
    private CollisionShape2D collider;
    private AudioStreamPlayer sfxAcquire;
    public override void _Ready()
    {
        area = GetNode<Area2D>("Area2D");
        collider = area.GetNode<CollisionShape2D>("CollisionShape2D");
        sfxAcquire = GetNode<AudioStreamPlayer>("SfxAcquire");
        
    }

    public override void _Process(float delta)
    {
        Translate(Vector2.Down * delta * speed);

        if(Position.y > 900)
            QueueFree();
    }

    public void Acquire()
    {
        sfxAcquire.Play();
        switch(type)
        {
            case "AttackUp":
                EmitSignal(nameof(AttackUpAcquired));
                break;
            case "FireRateUp":
                EmitSignal(nameof(FireRateUpAcquired));
                break;
            case "SpeedUp":
                EmitSignal(nameof(SpeedUpAcquired));
                break;
        }

        Visible = false;
        collider.SetDeferred("disabled",true);
    }

}
