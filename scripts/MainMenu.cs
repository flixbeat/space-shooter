using Godot;
using System;

public class MainMenu : Node2D
{
    private Button btnStart;
    private Button btnQuit;
    private Label labelHighScore;
    private AudioStreamPlayer bgm;

    public override void _Ready()
    {
        btnStart = GetNode<Button>("Control/VBoxContainer/BtnStart");
        btnQuit = GetNode<Button>("Control/VBoxContainer/BtnQuit");
        labelHighScore = GetNode<Label>("Control/LabelHighScore");
        bgm = GetNode<AudioStreamPlayer>("BGM");

        btnStart.Connect("pressed", this, nameof(OnBtnStartPressed));
        btnQuit.Connect("pressed", this, nameof(OnBtnQuitPressed));

        UpdateHighScore();

        Engine.TimeScale = 1;
        bgm.Play();
    }

    private void OnBtnStartPressed()
    {
        GetTree().ChangeScene("res://Space.tscn");
    }

    private void OnBtnQuitPressed()
    {
        GetTree().Quit();
    }

    private void UpdateHighScore()
    {
        labelHighScore.Text = $"High score: {Utils.LoadHighScore()}";
    }
}
