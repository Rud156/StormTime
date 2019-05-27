using Godot;
using System;

public class EnemySpawnPoint : Node2D
{
    [Export] public int maxEnemyDangerLevel;

    public int GetEnemyDangerLevel() => maxEnemyDangerLevel;
}
