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

    // Debug
    [Export] public bool isDebug;

    protected enum EnemyState
    {
        Idling,
        Wandering,
        Targeting,
        Attacking,
        Dead,
    }

    protected EnemyState _enemyState;

    // General States
    protected Vector2 _startPosition;
    protected Vector2 _targetPosition;

    // Idle States
    protected float idleTimeLeft;


    // Player Attacking State
    protected Vector2 _positionBeforePlayerFollow;

    public override void _Ready()
    {
        SetEnemyState(EnemyState.Idling);

        _startPosition = GetPosition();
        _positionBeforePlayerFollow = _startPosition;

        _targetPosition = GetNewPositionForIdling();
    }

    public override void _Process(float delta)
    {
        switch (_enemyState)
        {
            case EnemyState.Idling:
                break;

            case EnemyState.Wandering:
                break;

            case EnemyState.Targeting:
                break;

            case EnemyState.Attacking:
                break;

            case EnemyState.Dead:
                break;
        }
    }

    protected void UpdateIdling()
    {

    }

    protected void UpdateWandering()
    {

    }

    #region Utility Functions

    protected void MoveToTowardsTarget(Vector2 targetPosition, float delta)
    {
        Vector2 directionVector = (targetPosition - GetPosition()).Normalized();
        MoveAndSlide(directionVector * movementSpeed * delta);
    }

    protected void LookAtTarget(Vector2 target) => LookAt(target);

    protected Vector2 GetNewPositionForIdling() => VectorHelpers.Random2D() * explorationRadius;

    protected void SetEnemyState(EnemyState enemyState) => _enemyState = enemyState;

    #endregion
}
