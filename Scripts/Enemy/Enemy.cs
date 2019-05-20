using Godot;
using StormTime.Utils;
using System;

public abstract class Enemy : KinematicBody2D
{
    // Enemy State
    [Export] public float explorationRadius;
    [Export] public float idleTime;

    // Target Player Stats
    [Export] public float playerTargetDistance;
    [Export] public float maxPlayerFollowDistance;
    [Export] public float movementSpeed;

    protected enum EnemyState
    {
        Idling,
        Wandering,
        Targeting,
        Attacking,
        Dead,
    }

    protected EnemyState _enemyState;

    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private Vector2 _positionBeforePlayerFollow;

    public override void _Ready()
    {
        SetEnemyState(EnemyState.Idling);

        _startPosition = GetPosition();
        _positionBeforePlayerFollow = _startPosition;

        _targetPosition = GetNewPositionForIdling();
    }

    public override void _Process(float delta)
    {
        
    }

    protected Vector2 GetNewPositionForIdling() => VectorHelpers.Random2D() * explorationRadius;

    protected void SetEnemyState(EnemyState enemyState) => _enemyState = enemyState;
}
