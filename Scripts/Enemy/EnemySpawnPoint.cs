using Godot;
using System;

namespace StormTime.Enemy
{
    public class EnemySpawnPoint : Sprite
    {
        [Export] public int maxEnemyDangerLevel;

        public int GetEnemyDangerLevel() => maxEnemyDangerLevel;
    }
}