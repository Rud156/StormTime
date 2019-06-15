using System.Collections.Generic;
using Godot;

namespace StormTime.Enemy.Groups
{
    public class EnemyGroup : Node2D
    {
        [Export] public NodePath interactionParticlesNodePath;
        [Export] public NodePath interactionSpriteNodePath;
        [Export] public NodePath enemyPlayerInteractionNodePath;
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
        private List<int> _enemyTypeCount;
        private List<Individuals.Enemy> _groupEnemies;

        private EnemyGroupPlayerInteraction _enemyGroupPlayerInteraction;
        private Particles2D _interactionParticles;
        private Sprite _interactionSprite;
        private GradientTexture _spawnGradientTexture;

        private int _currentEnemyTypeIndex;
        private int _currentEnemyTypeCount;
        private bool _spawnEnemies;

        private bool _playerHostile;

        public override void _Ready()
        {
            _spawnPoints = new List<Node2D>();
            _enemyTypeCount = new List<int> { 0, 0, 0 };
            _groupEnemies = new List<Individuals.Enemy>();

            _enemyGroupPlayerInteraction = GetNode<EnemyGroupPlayerInteraction>(enemyPlayerInteractionNodePath);

            _interactionSprite = GetNode<Sprite>(interactionSpriteNodePath);
            _interactionParticles = GetNode<Particles2D>(interactionParticlesNodePath);

            foreach (NodePath spawnPoint in spawnPointsNodePaths)
            {
                _spawnPoints.Add(GetNode<Node2D>(spawnPoint));
            }
        }

        public override void _Process(float delta)
        {
            if (!_spawnEnemies)
            {
                return;
            }

            CountAndSpawnEnemies();
        }

        #region Utility Functions

        private void StartEnemiesSpawn()
        {
            _spawnEnemies = true;
            _currentEnemyTypeIndex = 0;
            _currentEnemyTypeCount = 0;
        }

        private void CountAndSpawnEnemies()
        {
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

            SpawnEnemy();

            _currentEnemyTypeCount += 1;
            if (_currentEnemyTypeCount >= _enemyTypeCount[_currentEnemyTypeIndex])
            {
                _currentEnemyTypeIndex += 1;
                _currentEnemyTypeCount = 0;
            }
        }

        private void SpawnEnemy()
        {
            Individuals.Enemy enemyInstance = (Individuals.Enemy)enemyTypes[_currentEnemyTypeIndex].Instance();
            AddChild(enemyInstance);
            _groupEnemies.Add(enemyInstance);

            Node2D spawnPoint = _spawnPoints[(int)(GD.Randi() % _spawnPoints.Count)];
            enemyInstance.SetGlobalPosition(spawnPoint.GetGlobalPosition());
            enemyInstance.SetEnemyColors(
                enemyColors[GD.Randi() % enemyColors.Length],
                enemyColors[GD.Randi() % enemyColors.Length]
            );
            enemyInstance.SetParentEnemyGroup(this);
        }

        #endregion

        #region External Functions

        public void SetEnemyGroupColors(Color spriteColor, GradientTexture particlesGradient)
        {
            _interactionSprite.SelfModulate = spriteColor;
            _interactionParticles.ProcessMaterial.Set("color_ramp", particlesGradient);

            _spawnGradientTexture = particlesGradient;
        }

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

        public GradientTexture GetGroupGradientTexture() => _spawnGradientTexture;

        public void SetPlayerAsHostile(bool playerHostile = true)
        {
            foreach (Individuals.Enemy enemy in _groupEnemies)
            {
                enemy.SetPlayerHostileState(playerHostile);
            }

            _playerHostile = playerHostile;
        }

        public bool IsPlayerHostile() => _playerHostile;

        #endregion
    }
}