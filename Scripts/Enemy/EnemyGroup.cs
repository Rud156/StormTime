using System;
using Godot;

namespace StormTime.Enemy
{
    public class EnemyGroup : Node2D
    {
        [Export] public Godot.Collections.Array<Color> enemyColors;
        [Export] public Godot.Collections.Array<PackedScene> enemyTypes;
        [Export] public int[] enemyTypeCount;
        [Export] public Vector2[] spawnPoints;

        private Vector2 _groupPosition;
        private int _currentEnemyTypeIndex;
        private int _currentEnemyTypeCount;
        private bool _spawnEnemies;

        public override void _Ready()
        {
            _groupPosition = GetPosition();
            StartEnemiesSpawn();
        }

        private void StartEnemiesSpawn()
        {
            _spawnEnemies = true;
            _currentEnemyTypeIndex = 0;
            _currentEnemyTypeCount = 0;
        }

        public override void _Process(float delta)
        {
            if (!_spawnEnemies)
            {
                return;
            }

            Enemy enemyInstance = (Enemy)enemyTypes[_currentEnemyTypeIndex].Instance();
            enemyInstance.SetPosition(spawnPoints[GD.Randi() % spawnPoints.Length]);
            enemyInstance.SetEnemyColors(
                enemyColors[(int)(GD.Randi() % enemyColors.Count)],
                enemyColors[(int)(GD.Randi() % enemyColors.Count)]
            );

            _currentEnemyTypeCount += 1;
            if (_currentEnemyTypeCount >= enemyTypeCount[_currentEnemyTypeIndex])
            {
                _currentEnemyTypeIndex += 1;
                _currentEnemyTypeCount = 0;
            }
            if (_currentEnemyTypeIndex >= enemyTypeCount.Length)
            {
                _spawnEnemies = false;
                _currentEnemyTypeIndex = 0;
                _currentEnemyTypeCount = 0;
            }
        }
    }
}