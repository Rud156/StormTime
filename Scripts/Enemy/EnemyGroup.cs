using System.Collections.Generic;
using Godot;

namespace StormTime.Enemy
{
    public class EnemyGroup : Node2D
    {
        [Export] public Color[] enemyColors;
        
        /// <summary>
        /// Currently Enemies Are:
        /// 1. Burst Enemy
        /// 2. Spinner Enemy
        /// 3. Slasher Enemy
        /// </summary>
        [Export] public Godot.Collections.Array<PackedScene> enemyTypes;
        [Export] public int[] enemyDangerValues;
        [Export] public Godot.Collections.Array<NodePath> spawnPointsNodePaths;

        private List<Node2D> _spawnPoints;

        private int _currentEnemyTypeIndex;
        private int _currentEnemyTypeCount;
        private bool _spawnEnemies;
        private List<int> _enemyTypeCount;

        public EnemyGroup()
        {
            _spawnPoints = new List<Node2D>();
            _enemyTypeCount = new List<int> {0, 0, 0};
        }

        public override void _Ready()
        {
            foreach (NodePath spawnPoint in spawnPointsNodePaths)
            {
                _spawnPoints.Add(GetNode<Node2D>(spawnPoint));
            }
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

            if (_currentEnemyTypeIndex >= _enemyTypeCount.Count)
            {
                _spawnEnemies = false;
                _currentEnemyTypeIndex = 0;
                _currentEnemyTypeCount = 0;
            }

            if (_enemyTypeCount[_currentEnemyTypeIndex] == 0)
            {
                _currentEnemyTypeIndex += 1;
                _currentEnemyTypeCount = 0;
                return;
            }

            Enemy enemyInstance = (Enemy)enemyTypes[_currentEnemyTypeIndex].Instance();
            Node2D spawnPoint = _spawnPoints[(int)(GD.Randi() % _spawnPoints.Count)];
            enemyInstance.SetPosition(spawnPoint.GetGlobalPosition());
            enemyInstance.SetEnemyColors(
                enemyColors[GD.Randi() % enemyColors.Length],
                enemyColors[GD.Randi() % enemyColors.Length]
            );
            AddChild(enemyInstance);
            
            _currentEnemyTypeCount += 1;
            if (_currentEnemyTypeCount >= _enemyTypeCount[_currentEnemyTypeIndex])
            {
                _currentEnemyTypeIndex += 1;
                _currentEnemyTypeCount = 0;
            }
        }

        #region External Functions

        public void ActivateEnemySpawning(int maxDangerAmount)
        {
            if (maxDangerAmount <= 0)
            {
                return;
            } 

            GD.Print($"Max Danger Amount: {maxDangerAmount}");
            int currentDangerAmount = 0;

            while (true)
            {
                int enemyIndex = (int)(GD.Randi() % enemyTypes.Count);
                int randomEnemyDangerValue = enemyDangerValues[enemyIndex];
                
                if (currentDangerAmount + randomEnemyDangerValue > maxDangerAmount)
                {
                    continue;
                }

                currentDangerAmount += randomEnemyDangerValue;
                _enemyTypeCount[enemyIndex] += 1;

                if (currentDangerAmount == maxDangerAmount)
                {
                    break;
                }
            }

            GD.Print("Created All Enemies. Wowser!!!");
            StartEnemiesSpawn();
        }

        #endregion
    }
}