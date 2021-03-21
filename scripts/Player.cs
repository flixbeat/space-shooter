using Godot;
using System;
using System.Threading.Tasks;

public class Player : Sprite
{
    [Signal] delegate void Exploded();
    [Export] private NodePath spacePath;
    [Export] private float moveSpeed = 200f;
    [Export] private float fireRateSec = 0.05f;
    [Export] private PackedScene[] laserScenes;
    private int laserIndex;
    private Node2D space;
    private Node2D muzzle;
    private Sprite explode;
    private Area2D area;
    private CollisionShape2D collider;
    private Timer timerDestroy;
    private float timePassed;
    private AudioStreamPlayer sfxShoot;
    private AudioStreamPlayer sfxExplode;

    public override void _Ready()
    {
        muzzle = GetNode<Node2D>("Muzzle");
        space = GetNode<Node2D>(spacePath);
        explode = GetNode<Sprite>("Explode");
        area = GetNode<Area2D>("Area2D");
        collider = area.GetNode<CollisionShape2D>("CollisionShape2D");
        timerDestroy = GetNode<Timer>("TimerDestroy");
        sfxShoot = GetNode<AudioStreamPlayer>("SfxShoot");
        sfxExplode = GetNode<AudioStreamPlayer>("SfxExplode");
        
        area.Connect("area_entered", this, nameof(OnAreaEntered));
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("move_left"))
            Translate(Vector2.Left * delta * moveSpeed);
        else if (Input.IsActionPressed("move_right"))
            Translate(Vector2.Right * delta  * moveSpeed);

        Position = new Vector2(Mathf.Clamp(Position.x,30,370), Position.y);

        if(Input.IsActionPressed("fire"))
        {
            if(timePassed >= fireRateSec)
            {
                Laser laser = (Laser) laserScenes[laserIndex].Instance();
                laser.GlobalPosition = muzzle.GlobalPosition;
                space.AddChild(laser);
                sfxShoot.Play();
                timePassed = 0;
            }
        }
        
        timePassed += delta;
    }

    private void OnAreaEntered(Area2D other)
    {
        Node parent = other.GetParent();
        if(parent.Name.Contains("PowerUp"))
        {
            PowerUp powerUp = (PowerUp) parent;
            powerUp.Connect("AttackUpAcquired", this, nameof(OnAttackUpAcquired));
            powerUp.Connect("FireRateUpAcquired", this, nameof(OnFireRateUpAcquired));
            powerUp.Connect("SpeedUpAcquired", this, nameof(OnSpeedUpAcquired));
            powerUp.Acquire();
        }
        else
            Destroy();
    }

    private void Destroy()
    {
        SetProcess(false);
        SelfModulate = new Color(0,0,0,0);
        collider.SetDeferred("disabled", true);
        timerDestroy.Start();

        explode.Visible = true;
        explode.GetNode<AnimationPlayer>("AnimationPlayer").Play("default");

        sfxExplode.Play();

        EmitSignal(nameof(Exploded));
    }

    public async void Respawn()
    {
        await Task.Delay(2500);
        SetProcess(true);
        
        await Flicker();
        collider.SetDeferred("disabled", false);
    }

    private async Task Flicker()
    {
        for(int i = 0 ; i < 50; i++)
        {
            SelfModulate = new Color(1,1,1,.25f);
            await Task.Delay(20);
            SelfModulate = new Color(1,1,1,1);
            await Task.Delay(20);
        }
    }

    private void OnAttackUpAcquired()
    {
        if(laserIndex < laserScenes.Length-1)
            laserIndex+=1;
    }

    private void OnFireRateUpAcquired()
    {
        if(fireRateSec > 0.2)
            fireRateSec -= 0.05f;
    }

    private void OnSpeedUpAcquired()
    {
        if(moveSpeed < 400)
            moveSpeed += 20;
    }
}
