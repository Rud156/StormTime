using Godot;
using StormTime.Player.Data;
using StormTime.Utils;
using System;

namespace StormTime.Enemy
{
    public abstract class Enemy : KinematicBody2D
    {
        // Enemy State
        [Export] public float explorationRadius;
        [Export] public float idleTime;
        [Export] public float minWanderingReachDistance;

        // Target Player Stats
        [Export] public float playerTargetDistance;
        [Export] public float playerAttackDistance;
        [Export] public float maxPlayerFollowDistance;
        [Export] public float movementSpeed;

        // Target Attack Stats
        [Export] public float attackTime;

        // Debug
        [Export] public bool isDebug;

        protected enum EnemyState
        {
            Homing,
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

        // Timer
        protected float _enemyTimer;

        public override void _Ready()
        {
            _startPosition = GetPosition();
            _enemyTimer = 0;

            SetEnemyState(EnemyState.Idling);
        }

        public override void _Process(float delta)
        {
            OverHeadCheckForEnemyState();

            switch (_enemyState)
            {
                case EnemyState.Homing:
                    UpdateHoming(delta);
                    break;

                case EnemyState.Idling:
                    UpdateIdling(delta);
                    break;

                case EnemyState.Wandering:
                    UpdateWandering(delta);
                    break;

                case EnemyState.Targeting:
                    UpdateTargeting(delta);
                    break;

                case EnemyState.Attacking:
                    UpdateAttacking(delta);
                    break;

                case EnemyState.Dead:
                    UpdateDead(delta);
                    break;
            }
        }

        private void OverHeadCheckForEnemyState()
        {
            if (GetPosition().DistanceSquaredTo(PlayerVariables.PlayerPosition) <= playerTargetDistance &&
            _enemyState != EnemyState.Attacking && _enemyState != EnemyState.Dead)
            {
                _enemyTimer = attackTime;
                SetEnemyState(EnemyState.Targeting);
            }

            if (GetPosition().DistanceSquaredTo(PlayerVariables.PlayerPosition) <= playerAttackDistance &&
            _enemyState != EnemyState.Attacking && _enemyState != EnemyState.Dead)
            {
                SetEnemyState(EnemyState.Attacking);
            }

            if (GetPosition().DistanceSquaredTo(_startPosition) > maxPlayerFollowDistance &&
            _enemyState == EnemyState.Targeting)
            {
                SetEnemyState(EnemyState.Homing);
            }

            if (GetPosition().DistanceSquaredTo(PlayerVariables.PlayerPosition) > playerTargetDistance &&
             _enemyState == EnemyState.Targeting)
            {
                SetEnemyState(EnemyState.Idling);
            }
        }

        protected void UpdateHoming(float delta)
        {
            _targetPosition = _startPosition;
            MoveToTowardsTarget(_targetPosition);


            if (GetPosition().DistanceSquaredTo(_targetPosition) <= minWanderingReachDistance)
            {
                _enemyTimer = idleTime;
                SetEnemyState(EnemyState.Idling);
            }
        }

        protected void UpdateIdling(float delta)
        {
            _enemyTimer -= delta;
            if (_enemyTimer <= 0)
            {
                Vector2 newIdlingTarget = GetNewPositionForIdling();
                _targetPosition = newIdlingTarget;
                SetEnemyState(EnemyState.Wandering);
            }
        }

        protected void UpdateWandering(float delta)
        {
            MoveToTowardsTarget(_targetPosition);

            if (GetPosition().DistanceSquaredTo(_targetPosition) <= minWanderingReachDistance)
            {
                _enemyTimer = idleTime;
                SetEnemyState(EnemyState.Idling);
            }
        }

        protected void UpdateTargeting(float delta)
        {
            _targetPosition = PlayerVariables.PlayerPosition;
            MoveToTowardsTarget(_targetPosition);
        }

        protected virtual void UpdateAttacking(float delta)
        {
            _enemyTimer -= delta;

            if (_enemyTimer <= 0)
            {
                SetEnemyState(EnemyState.Targeting);
            }
        }

        protected void UpdateDead(float delta)
        {
            // TODO: Play some effect or something here...
        }

        #region Utility Functions

        protected void MoveToTowardsTarget(Vector2 targetPosition)
        {
            Vector2 directionVector = (targetPosition - GetPosition()).Normalized();
            MoveAndSlide(directionVector * movementSpeed);
        }

        protected void LookAtTarget(Vector2 target) => LookAt(target);

        protected Vector2 GetNewPositionForIdling() => VectorHelpers.Random2D() * explorationRadius;

        protected void SetEnemyState(EnemyState enemyState) => _enemyState = enemyState;

        #endregion
    }
}