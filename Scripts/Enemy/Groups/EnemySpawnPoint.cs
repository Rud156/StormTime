using Godot;
using System;

namespace StormTime.Enemy.Groups
{
    public class EnemySpawnPoint : Node2D
    {
        [Export] public int maxEnemyDangerLevel;

        public int GetEnemyDangerLevel() => maxEnemyDangerLevel;
    }
}