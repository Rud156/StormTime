using System;
using Godot;
using System.Collections.Generic;
using StormTime.Player.Data;
using StormTime.Utils;

namespace StormTime.Enemy
{
    public abstract class Enemy : KinematicBody2D
    {
        // Enemy State
        [Export] public NodePath enemySpriteNodePath;
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
        [Export] public Godot.Collections.Array<NodePath> launchPointsPath;

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

        // Distances Squared
        protected float _playerTargetSqDst;
        protected float _playerAttackSqDst;
        protected float _maxPlayerFollowSqDst;

        // General States
        protected Vector2 _startPosition;
        protected Vector2 _targetPosition;

        // Timer
        protected float _enemyTimer;

        // Launch Points
        protected List<Node2D> _launchPoints;

        // Colors
        protected Color _enemyColor;
        protected Color _bulletColor;
        protected Sprite _enemySprite;

        public override void _Ready()
        {
            _launchPoints = new List<Node2D>();
            foreach (NodePath launchPoint in launchPointsPath)
            {
                _launchPoints.Add(GetNode<Node2D>(launchPoint));
            }

            _startPosition = GetGlobalPosition();
            _enemyTimer = 0;

            _playerTargetSqDst = playerTargetDistance * playerTargetDistance;
            _playerAttackSqDst = playerAttackDistance * playerAttackDistance;
            _maxPlayerFollowSqDst = maxPlayerFollowDistance * maxPlayerFollowDistance;

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
            if (GetGlobalPosition().DistanceSquaredTo(PlayerVariables.PlayerPosition) <= _playerTargetSqDst &&
            _enemyState != EnemyState.Attacking && _enemyState != EnemyState.Dead && _enemyState != EnemyState.Homing)
            {
                SetEnemyState(EnemyState.Targeting);
            }

            if (GetGlobalPosition().DistanceSquaredTo(PlayerVariables.PlayerPosition) <= _playerAttackSqDst &&
            _enemyState != EnemyState.Attacking && _enemyState != EnemyState.Dead)
            {
                LaunchAttack();
                SetEnemyState(EnemyState.Attacking);
            }

            if (GetGlobalPosition().DistanceSquaredTo(_startPosition) > _maxPlayerFollowSqDst &&
            _enemyState == EnemyState.Targeting)
            {
                SetEnemyState(EnemyState.Homing);
            }

            if (GetGlobalPosition().DistanceSquaredTo(PlayerVariables.PlayerPosition) > _playerTargetSqDst &&
             _enemyState == EnemyState.Targeting)
            {
                SetEnemyState(EnemyState.Idling);
            }
        }

        #region Enemy States Updates

        protected void UpdateHoming(float delta)
        {
            _targetPosition = _startPosition;
            MoveToTowardsTarget(_targetPosition);


            if (GetGlobalPosition().DistanceSquaredTo(_targetPosition) <= minWanderingReachDistance)
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

            if (GetGlobalPosition().DistanceSquaredTo(_targetPosition) <= minWanderingReachDistance)
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
                EndAttack();
                SetEnemyState(EnemyState.Targeting);
            }
        }

        protected virtual void LaunchAttack() => _enemyTimer = attackTime;

        protected virtual void EndAttack() => _enemyTimer = 0;

        protected void UpdateDead(float delta)
        {
            // TODO: Play some effect or something here...
        }

        #endregion

        #region Utility Functions

        public void SetEnemyColors(Color enemyColor, Color bulletColor)
        {
            _enemyColor = enemyColor;
            _bulletColor = bulletColor;

            _enemySprite = GetNode<Sprite>(enemySpriteNodePath);
            _enemySprite.SetSelfModulate(_enemyColor);
        }

        protected void MoveToTowardsTarget(Vector2 targetPosition)
        {
            Vector2 directionVector = (targetPosition - GetGlobalPosition()).Normalized();
            MoveAndSlide(directionVector * movementSpeed);
        }

        protected void MoveToTowardsTarget(Vector2 targetPosition, float speed)
        {
            Vector2 directionVector = (targetPosition - GetGlobalPosition()).Normalized();
            MoveAndSlide(directionVector * speed);
        }

        protected void LookAtTarget(Vector2 target) => LookAt(target);

        protected Vector2 GetNewPositionForIdling() => VectorHelpers.Random2D() * explorationRadius;

        protected void SetEnemyState(EnemyState enemyState)
        {
            if (_enemyState == enemyState)
            {
                return;
            }

            GD.Print($"Enemy State: {enemyState}, Enemy Name: {GetName()}");

            _enemyState = enemyState;
        }

        #endregion
    }
}