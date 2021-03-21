using Godot;
using System;

public class Wave : Resource
{
    [Export(PropertyHint.Range,"20,90")] public int time;
    [Export] public int meteorCount;
    [Export] public int enemyCount;
    [Export] public int powerUpCount;
    [Export] public PackedScene[] enemies;
    [Export] public PackedScene[] powerUps;
}