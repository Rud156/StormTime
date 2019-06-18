using System;
using Godot;
using System.Collections.Generic;
using Object = Godot.Object;

namespace StormTime.Enemy.Groups
{
    public class EnemySpawner : Node2D
    {
        [Export] public PackedScene enemyGroupPrefab;
        [Export] public int minEnemyGroupsToSpawn;
        [Export] public int maxEnemyGroupsToSpawn;
        [Export] public Godot.Collections.Array<Object> enemyGroupGradients;
        [Export] public Color[] enemyGroupColors;
        [Export] public Godot.Collections.Array<NodePath> worldSpawnNodePaths;

        private List<EnemySpawnPoint> _worldSpawnPoints;
        private List<EnemySpawnPoint> _availableWorldSpawnPoints;
        private List<EnemyGroup> _enemyGroupsSpawned;

        private int _enemyGroupsToSpawn;
        private int _currentEnemyGroups;
        private bool _startSpawn;

        public override void _Ready()
        {
            //GD.Randomize();

            _worldSpawnPoints = new List<EnemySpawnPoint>();
            _availableWorldSpawnPoints = new List<EnemySpawnPoint>();
            _enemyGroupsSpawned = new List<EnemyGroup>();
            _currentEnemyGroups = 0;
            _enemyGroupsToSpawn = (int)(GD.Randi() % (maxEnemyGroupsToSpawn - minEnemyGroupsToSpawn)) + minEnemyGroupsToSpawn;

            foreach (NodePath worldSpawnNodePath in worldSpawnNodePaths)
            {
                _worldSpawnPoints.Add(GetNode<EnemySpawnPoint>(worldSpawnNodePath));
            }

            _availableWorldSpawnPoints.AddRange(_worldSpawnPoints);

            GD.Print("Starting Enemy Group Spawn");
            _startSpawn = true;
            _currentEnemyGroups = 0;
        }

        public override void _Process(float delta)
        {
            if (!_startSpawn)
            {
                return;
            }

            if (_currentEnemyGroups >= _enemyGroupsToSpawn)
            {
                GD.Print("Enemy Group Spawning Complete");
                _startSpawn = false;
                _currentEnemyGroups = 0;
                return;
            }

            SpawnEnemyGroup();
            _currentEnemyGroups += 1;
        }

        private void SpawnEnemyGroup()
        {
            GD.Print($"Spawning Enemy Group: {_currentEnemyGroups}");

            int randomIndex = (int)(GD.Randi() % _availableWorldSpawnPoints.Count);
            EnemySpawnPoint spawnNode = _availableWorldSpawnPoints[randomIndex];
            Vector2 spawnPosition = spawnNode.GetGlobalPosition();

            EnemyGroup enemyGroupInstance = (EnemyGroup)enemyGroupPrefab.Instance();
            spawnNode.AddChild(enemyGroupInstance);
            _enemyGroupsSpawned.Add(enemyGroupInstance);

            enemyGroupInstance.SetEnemyGroupColors(
                enemyGroupColors[GD.Randi() % enemyGroupColors.Length],
                (GradientTexture)enemyGroupGradients[(int)(GD.Randi() % enemyGroupGradients.Count)]
            );

            enemyGroupInstance.ActivateEnemySpawning(spawnNode.GetEnemyDangerLevel());
            enemyGroupInstance.SetGlobalPosition(spawnPosition);

            _availableWorldSpawnPoints.RemoveAt(randomIndex);
        }
    }
}