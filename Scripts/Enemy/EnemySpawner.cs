using Godot;
using System.Collections.Generic;
using StormTime.Enemy;
using StormTime.Utils;

public class EnemySpawner : Node2D
{
    [Export] public NodePath enemyGroupsHolderNodePath;
    [Export] public PackedScene enemyGroupPrefab;
    [Export] public int minEnemyGroupsToSpawn;
    [Export] public int maxEnemyGroupsToSpawn;
    [Export] public Godot.Collections.Array<NodePath> worldSpawnNodePaths;

    private List<EnemySpawnPoint> _worldSpawnPoints;
    private List<EnemySpawnPoint> _availableWorldSpawnPoints;

    private Node2D _enemyGroupsHolder;

    private int _enemyGroupsToSpawn;
    private int _currentEnemyGroups;
    private bool _startSpawn;
    
    public override void _Ready()
    {
        _worldSpawnPoints = new List<EnemySpawnPoint>();
        _availableWorldSpawnPoints = new List<EnemySpawnPoint>();
        _enemyGroupsHolder = GetNode<Node2D>(enemyGroupsHolderNodePath);
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

        if (Input.IsActionJustPressed(SceneControls.Interact))
        {
            SpawnEnemyGroup();
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
        int randomIndex = (int) (GD.Randi() % _availableWorldSpawnPoints.Count);
        EnemySpawnPoint spawnNode = _availableWorldSpawnPoints[randomIndex];
        Vector2 spawnPosition = spawnNode.GetGlobalPosition();

        EnemyGroup enemyGroupInstance = (EnemyGroup) enemyGroupPrefab.Instance();
        enemyGroupInstance.ActivateEnemySpawning(spawnNode.GetEnemyDangerLevel());
        enemyGroupInstance.SetGlobalPosition(spawnPosition);
        
        _enemyGroupsHolder.AddChild(enemyGroupInstance);
        _availableWorldSpawnPoints.RemoveAt(randomIndex);
    }
}
