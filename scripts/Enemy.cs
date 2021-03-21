using Godot;
using System;
using System.Threading.Tasks;

public class Enemy : Sprite
{    
    [Signal] delegate void Exploded();
    [Export] NodePath spacePath;
    [Export] public int score;
    [Export] private int hp = 7;
    [Export] private float speed = 50f;
    [Export] private bool isFiring;
    [Export] private PackedScene enemyBulletScene;
    private Vector2 direction;
    private Timer timerChangeDir;
    private Area2D area;
    private CollisionShape2D collider;
    private Timer timerDestroy;
    private Sprite explode;
    private Timer timerFire;
    private Node2D space;
    private Node2D muzzle;
    private AudioStreamPlayer sfxHit;
    private AudioStreamPlayer sfxExplode;
    private AudioStreamPlayer sfxShoot;
    public override void _Ready()
    {
        timerChangeDir = GetNode<Timer>("TimerChangeDirection");
        area = GetNode<Area2D>("Area2D");
        timerDestroy = GetNode<Timer>("TimerDestroy");
        explode = GetNode<Sprite>("Explode");
        collider = area.GetNode<CollisionShape2D>("CollisionShape2D");
        timerFire = GetNode<Timer>("TimerFire");
        space = GetNode<Node2D>(spacePath);
        muzzle = GetNode<Node2D>("Muzzle");
        sfxHit = GetNode<AudioStreamPlayer>("SfxHit");
        sfxExplode = GetNode<AudioStreamPlayer>("SfxExplode");
        sfxShoot = GetNode<AudioStreamPlayer>("SfxShoot");

        direction = Vector2.Down;
    
        timerChangeDir.Connect("timeout", this, nameof(OnChangeDirTimedOut));
        timerDestroy.Connect("timeout", this, nameof(Destroy));
        area.Connect("area_entered", this, nameof(OnAreaEntered));
        timerFire.Connect("timeout", this, nameof(OnFireTimedOut));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Translate(direction * delta * speed);

        Position = new Vector2(Mathf.Clamp(Position.x,30,370), Position.y);

        if(Position.y > 900)
            QueueFree();
    }

    private void OnChangeDirTimedOut()
    {
        int dir = Utils.RandomNext(0,3);
        switch(dir)
        {
            case 0:
                direction = new Vector2(-1,1);
                break;
            case 1:
                direction = new Vector2(1,1);
                break;
            case 2:
                direction = Vector2.Down;
                break;
        }            
    }

    private void OnAreaEntered(Area2D other)
    {
        Node parent = other.GetParent();
        if(parent.Name.Contains("Laser"))
        {
            Laser laser = (Laser) parent;
            Damage(laser);
        }
        else
        {
            Destroy();
        }
    }

    private async void Damage(Laser laser)
    {
        sfxHit.Play();
        hp -= laser.damage;
        await ChangeColor();
        
        if(hp <= 0)
        {
            SetProcess(false);
            SelfModulate = new Color(0,0,0,0);
            collider.SetDeferred("disabled",true);
            explode.Visible = true;
            explode.GetNode<AnimationPlayer>("AnimationPlayer").Play("default");
            timerDestroy.Start();
            sfxExplode.Play();
            EmitSignal("Exploded");
        }
    }

    private async Task ChangeColor()
    {
        Modulate = new Color(10,10,10,1);
        await Task.Delay(10);
        Modulate = new Color(1,1,1,1);
    }

    private void Destroy()
    {
        QueueFree();
    }

    private void OnFireTimedOut()
    {
        if(!isFiring) return;

        EnemyBullet bullet = (EnemyBullet) enemyBulletScene.Instance();
        bullet.GlobalPosition = muzzle.GlobalPosition;
        space.AddChild(bullet);

        sfxShoot.Play();
    }

}
