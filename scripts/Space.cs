using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
public class Space : Node2D
{
    [Export(PropertyHint.Range,"0,10")] int playerLife = 3;
    private int score;
    [Export] Resource[] waves;
    [Export] PackedScene meteorScene;
    private PackedScene[] powerUpScenes;
    private PackedScene[] enemyScenes;
    private Timer timerMeteorSpawner;
    private Timer timerPowerUpSpawner;
    private Timer timerEnemySpawner;
    private Timer timerWave;
    private Timer timerWaveCooldown;
    private int currentWaveCount;
    private Wave wave;
    private Player player;
    private LifeContainer lifeContainer;
    private Label labelWave;
    private Label labelScore;
    private Label labelWaveShow;
    private Label labelGameOver;
    private Label labelCurrentScore;
    private Label labelHighScore;
    private AudioStreamPlayer bgm;
    public override void _Ready()
    {
        timerMeteorSpawner = GetNode<Timer>("TimerMeteorSpawner");
        timerPowerUpSpawner = GetNode<Timer>("TimerPowerUpSpawner");
        timerEnemySpawner = GetNode<Timer>("TimerEnemySpawner");
        timerWave = GetNode<Timer>("TimerWave");
        timerWaveCooldown = GetNode<Timer>("TimerWaveCooldown");
        player = GetNode<Player>("Player");
        lifeContainer = GetNode<LifeContainer>("Canvas/Stats/PanelContainer/HBoxContainer/ColorRect/LifeContainer");
        labelWave = GetNode<Label>("Canvas/Stats/PanelContainer/HBoxContainer/ColorRect2/WaveScoreContainer/Wave");
        labelScore = GetNode<Label>("Canvas/Stats/PanelContainer/HBoxContainer/ColorRect2/WaveScoreContainer/Score");
        labelWaveShow = GetNode<Label>("Canvas/LabelWave");
        labelGameOver = GetNode<Label>("Canvas/LabelGameOver");
        labelCurrentScore = GetNode<Label>("Canvas/LabelGameOver/LabelCurrentScore");
        labelHighScore = GetNode<Label>("Canvas/LabelGameOver/LabelHighScore");
        bgm = GetNode<AudioStreamPlayer>("BGM");

        timerMeteorSpawner.Connect("timeout", this, nameof(OnTimerMeteorTimeout));
        timerPowerUpSpawner.Connect("timeout", this, nameof(OnTimerPowerUpTimeout));
        timerEnemySpawner.Connect("timeout", this, nameof(OnTimerEnemyTimeout));
        timerWave.Connect("timeout", this, nameof(OnTimerWaveTimeout));
        timerWaveCooldown.Connect("timeout", this, nameof(OnTimerWaveCooldownTimeout));
        player.Connect("Exploded", this, nameof(OnExploded));

        SetProcessInput(false);
        SetupWave();
        InitializePlayerLife();
        ShowWaveText();
        
        Engine.TimeScale = 1;
        bgm.Play();
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey ev)
        {
            if(ev.IsPressed())
                 GetTree().ChangeScene("res://MainMenu.tscn");
        }
    }

    private void InitializePlayerLife()
    {
        for(int i = 0; i < playerLife ; i++)
            lifeContainer.AddLife();
    }

    private void SetupWave()
    {
        wave = (Wave) waves[currentWaveCount];
        float meteorSpawnWaitTime = wave.time / wave.meteorCount;
        float enemySpawnWaitTime = wave.time / wave.enemyCount;
        float powerSpawnWaitTime = wave.time / wave.powerUpCount;

        powerUpScenes = wave.powerUps;
        enemyScenes = wave.enemies;

        timerMeteorSpawner.WaitTime = meteorSpawnWaitTime;
        timerEnemySpawner.WaitTime = enemySpawnWaitTime;
        timerPowerUpSpawner.WaitTime = powerSpawnWaitTime;
        timerWave.WaitTime = wave.time;

        timerMeteorSpawner.Start();
        timerEnemySpawner.Start();
        timerPowerUpSpawner.Start();
        timerWave.Start();

        UpdateWave();

        GD.Print("====== wave info ======");
        GD.Print($"wave: {currentWaveCount+1}");
        GD.Print($"wave time: {wave.time}");
        GD.Print($"meteor spawn time: {meteorSpawnWaitTime}");
        GD.Print($"enemy spawn time: {enemySpawnWaitTime}");
        GD.Print($"power up spawn time: {powerSpawnWaitTime}");
    }

    private void OnTimerMeteorTimeout()
    {
        Meteor meteor = (Meteor) meteorScene.Instance();
        float x = Utils.RandomRange(30,370);
        meteor.GlobalPosition = new Vector2(x,-30);
        Array binds = new Array();
        binds.Add(meteor);
        meteor.Connect("Exploded", this, nameof(OnMeteorExploded),binds);
        AddChild(meteor);
    }

    private void OnTimerPowerUpTimeout()
    {
        int i = Utils.RandomNext(0, powerUpScenes.Length);
        PowerUp powerUp = (PowerUp) powerUpScenes[i].Instance();
        float x = Utils.RandomRange(30,370);
        powerUp.GlobalPosition = new Vector2(x,-30);
        AddChild(powerUp);
    }

    private void OnTimerEnemyTimeout()
    {
        int i = Utils.RandomNext(0, enemyScenes.Length);
        Enemy enemy = (Enemy) enemyScenes[i].Instance();
        float x = Utils.RandomRange(30,370);
        enemy.GlobalPosition = new Vector2(x,-30);
        Array binds = new Array();
        binds.Add(enemy);
        enemy.Connect("Exploded", this, nameof(OnEnemyExploded), binds);
        AddChild(enemy);
    }

    private void OnTimerWaveTimeout()
    {
        GD.Print("wave over");
        timerEnemySpawner.Stop();
        timerMeteorSpawner.Stop();
        timerPowerUpSpawner.Stop();
        timerWaveCooldown.Start();
        GD.Print("entering wave cooldown");
    }

    private void OnTimerWaveCooldownTimeout()
    {
        currentWaveCount+=1;
        SetupWave();
        ShowWaveText();
    }

    private void OnExploded()
    {
        if(lifeContainer.GetLife() > 0)
        {
            player.Respawn();
            lifeContainer.DeductLife();
        }
        else
            ShowGameOver();
    }

    private void OnMeteorExploded(Meteor meteor)
    {
        AddScore(meteor.score);
    }

    private void OnEnemyExploded(Enemy enemy)
    {
        AddScore(enemy.score);
    }

    private void AddScore(int value)
    {
        score += value;
        UpdateScore();
    }

    private void AddWave()
    {
        currentWaveCount += 1;
        UpdateWave();
    }

    private void UpdateScore()
    {
        labelScore.Text = $"Score: {score}";
    }

    private void UpdateWave()
    {
        labelWave.Text = $"Wave: {currentWaveCount+1}";
    }

    private async void ShowWaveText()
    {
        labelWaveShow.Text = $"Wave {currentWaveCount+1}";
        labelWaveShow.Visible = true;
        await Task.Delay(5000);
        labelWaveShow.Visible = false;
    }

    private async void ShowGameOver()
    {
        await Task.Delay(3000);
        labelGameOver.Visible = true;
        labelWaveShow.Visible = false;
        SetProcessInput(true);

        if(score > Utils.LoadHighScore())
            Utils.SaveHighScore(score);

        labelCurrentScore.Text = $"Your score: {score}";
        labelHighScore.Text = $"High score: {Utils.LoadHighScore()}";

        Engine.TimeScale = 0;
    }

}
