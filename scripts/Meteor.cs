using Godot;
using System;
using System.Threading.Tasks;

public class Meteor : Sprite
{
    [Signal] delegate void Exploded();
    [Export] public int score = 10;
    [Export] PackedScene meteorParticleScene;
    private Area2D area;
    private CollisionShape2D collider;
    private Timer timerDestroy;
    private AudioStreamPlayer sfxHit;
    private AudioStreamPlayer sfxExplode;
    [Export] public int health = 5;
    private float speed = 50f;
    private float rotationSpeed = 3f;
    private int particlesCount;
    public override void _Ready()
    {
        area = GetNode<Area2D>("Area2D");
        collider = area.GetNode<CollisionShape2D>("CollisionShape2D");
        timerDestroy = GetNode<Timer>("TimerDestroy");
        sfxHit = GetNode<AudioStreamPlayer>("SfxHit");
        sfxExplode = GetNode<AudioStreamPlayer>("SfxExplode");

        Random rand = new Random();
        speed = rand.Next(40,100);
        rotationSpeed = rand.Next(1,7);
        particlesCount = rand.Next(4,7);

        area.Connect("area_entered", this, nameof(OnAreaEntered));
        timerDestroy.Connect("timeout", this, nameof(RemoveFromScene));
    }

    public override void _Process(float delta)
    {
        Translate(Vector2.Down * delta * speed);
        Rotate(rotationSpeed * delta);

        if(Position.y > 900)
            QueueFree();
    }

    private void OnAreaEntered(Area2D other)
    {
        Node node = other.GetParent();
        
        if(node.Name.Contains("Laser"))
        {
            Laser laser = (Laser) node;
            Damage(laser);
        }
    }


    private async void Damage(Laser laser)
    {
        health -= laser.damage;
        sfxHit.Play();
        await ChangeColor();

        if(health <= 0)
        {
            SetProcess(false);
            SelfModulate = new Color(0,0,0,0);
            collider.SetDeferred("disabled",true);
            timerDestroy.Start();
            SpawnParticles();
            sfxExplode.Play();
            EmitSignal(nameof(Exploded));
        }
    }

    private void SpawnParticles()
    {
        for(int i = 0; i < particlesCount; i ++)
        {
            Node meteorParticle = meteorParticleScene.Instance();
            AddChild(meteorParticle);
        }
    }

    private async Task ChangeColor()
    {
        Modulate = new Color(10,10,10,1);
        await Task.Delay(10);
        Modulate = new Color(1,1,1,1);
    }

    private void RemoveFromScene()
    {
        QueueFree();
    }
}
