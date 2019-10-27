using Godot;
using System.Collections.Generic;
using StormTime.Player.Data;
using StormTime.Utils;
using StormTime.Enemy.Groups;
using StormTime.Common;
using StormTime.Effects;
using StormTime.Weapon;
using System;

namespace StormTime.Enemy.Individuals
{
    public abstract class Enemy : KinematicBody2D
    {
        // Enemy State
        [Export] public NodePath enemySpriteNodePath;
        [Export] public NodePath rotationNodePath;
        [Export] public NodePath enemyHealthSetterNodePath;
        [Export] public PackedScene enemyDeathEffectPrefab;
        [Export] public float explorationRadius;
        [Export] public float idleTime;
        [Export] public float minWanderingReachDistance;

        // Freezing Affector
        [Export] public float enemyFreezeTime;
        [Export] public float enemyFreezeRatio;

        // Target Player Stats
        [Export] public float playerTargetDistance;
        [Export] public float playerAttackDistance;
        [Export] public float maxPlayerFollowDistance;
        [Export] public float movementSpeed;

        // Target Attack Stats
        [Export] public float attackTime;
        [Export] public float targetingAttackTime = 1;
        [Export] public Godot.Collections.Array<NodePath> launchPointsPath;

        // Death Items
        [Export] public PackedScene soulsPrefab;
        [Export] public float minSoulsAmount;
        [Export] public float maxSoulsAmount;
        [Export] public float rangeMultiplier;

        protected enum EnemyState
        {
            Homing,
            Idling,
            Wandering,
            Targeting,
            Attacking,
            Dead,
            Frozen
        }

        protected EnemyState _enemyState;
        protected EnemyGroup _parentEnemyGroup;

        // Distances Squared
        protected float _playerTargetSqDst;
        protected float _playerAttackSqDst;
        protected float _maxPlayerFollowSqDst;

        // General States
        protected Vector2 _startPosition;
        protected Vector2 _targetPosition;
        protected bool _playerHostile;

        // Frozen Variables
        protected float _frozenTimer;
        protected EnemyState _previousStateBeforeFreezing;

        // Timer
        protected float _enemyTimer;

        // Launch Points and Rotation
        protected List<Node2D> _launchPoints;
        protected Node2D _rotationNode;

        // Colors
        protected Color _enemyColor;
        protected Color _bulletColor;
        protected Sprite _enemySprite;

        protected HealthSetter _enemyHealthSetter;
        protected int _currentSoulCount;

        public override void _Ready()
        {
            _rotationNode = GetNode<Node2D>(rotationNodePath);
            _enemyHealthSetter = GetNode<HealthSetter>(enemyHealthSetterNodePath);
            _enemyHealthSetter.zeroHealth += HandleHealthZero;

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

        public override void _ExitTree() => _enemyHealthSetter.zeroHealth -= HandleHealthZero;

        public override void _Process(float delta)
        {
            if (_enemyState != EnemyState.Frozen)
            {
                OverHeadCheckForEnemyState();
            }

            if (_enemyState == EnemyState.Targeting || _enemyState == EnemyState.Homing || _enemyState == EnemyState.Wandering)
            {
                OrientEnemyToTarget(delta, _targetPosition);
            }

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

                case EnemyState.Frozen:
                    UpdateFrozenEnemy(delta);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_enemyState), _enemyState, null);
            }
        }

        private void OverHeadCheckForEnemyState()
        {
            if (_enemyState == EnemyState.Dead)
            {
                return;
            }

            if (_playerHostile)
            {
                if (GetGlobalPosition().DistanceSquaredTo(PlayerVariables.LastPlayerPosition) <= _playerTargetSqDst &&
                    _enemyState != EnemyState.Attacking && _enemyState != EnemyState.Dead &&
                    _enemyState != EnemyState.Homing)
                {
                    SetEnemyState(EnemyState.Targeting);
                }

                if (GetGlobalPosition().DistanceSquaredTo(PlayerVariables.LastPlayerPosition) <= _playerAttackSqDst &&
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

                if (GetGlobalPosition().DistanceSquaredTo(PlayerVariables.LastPlayerPosition) > _playerTargetSqDst &&
                    _enemyState == EnemyState.Targeting)
                {
                    SetEnemyState(EnemyState.Idling);
                }
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

                // This is because Idling Position will be calculated with respect to the start position
                newIdlingTarget += _startPosition; // Otherwise all enemies will group up

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
            _targetPosition = PlayerVariables.LastPlayerPosition;
            MoveToTowardsTarget(_targetPosition);

            _enemyTimer -= delta;
            if (_enemyTimer <= 0)
            {
                _enemyTimer = targetingAttackTime;
                EnemyLaunchSingleShotAttack();
            }
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

        protected void UpdateFrozenEnemy(float delta)
        {
            _frozenTimer -= delta;

            if (_frozenTimer <= 0)
            {
                SetEnemyState(_previousStateBeforeFreezing);
            }
        }

        protected virtual void LaunchAttack() => _enemyTimer = attackTime;

        protected virtual void EndAttack() => _enemyTimer = 0;

        protected virtual void EnemyLaunchSingleShotAttack() { }

        protected virtual void OrientEnemyToTarget(float delta, Vector2 targetPosition) { }

        protected void UpdateDead(float delta)
        {
            SpawnSoul();
            _currentSoulCount -= 1;

            if (_currentSoulCount <= 0)
            {

                EnemyDeathParticleCleaner deathEffectInstance =
                    (EnemyDeathParticleCleaner)enemyDeathEffectPrefab.Instance();
                GetParent().AddChild(deathEffectInstance);

                deathEffectInstance.SetEffectGradient(_parentEnemyGroup.GetGroupGradientTexture());
                deathEffectInstance.SetGlobalPosition(GetGlobalPosition());

                RemoveEnemyFromWorld();
            }
        }

        #endregion

        #region External Functions

        // Event Function from Bullet Collision
        public void BulletCollisionNotification(object bullet, bool isFreezingBullet)
        {
            bool isPlayerBullet = !(bullet is EnemyBullet);

            if (isPlayerBullet)
            {
                EmitOnHostileOnPlayerAttack();

                float damageAmount = ((Bullet)bullet).GetBulletDamage();
                _enemyHealthSetter.SubtractHealth(damageAmount);
            }

            if (isFreezingBullet)
            {
                float randomNumber = (float)GD.Randf();
                if (randomNumber < enemyFreezeRatio)
                {
                    FreezeEnemy(enemyFreezeTime);
                }
            }
        }

        public void FreezeEnemy(float freezeTime)
        {
            _frozenTimer = freezeTime;
            _previousStateBeforeFreezing = _enemyState;

            SetEnemyState(EnemyState.Frozen);
        }

        public void SetEnemyColors(Color enemyColor, Color bulletColor)
        {
            _enemyColor = enemyColor;
            _bulletColor = bulletColor;

            _enemySprite = GetNode<Sprite>(enemySpriteNodePath);
            _enemySprite.SetSelfModulate(_enemyColor);
        }

        public void SetParentEnemyGroup(EnemyGroup enemyGroup) =>
            _parentEnemyGroup = enemyGroup;

        public void SetPlayerHostileState(bool playerHostile = true) => _playerHostile = playerHostile;

        public void EmitOnHostileOnPlayerAttack()
        {
            // _parentEnemyGroup.SetPlayerAsHostile();

            SetPlayerHostileState(true);
        }

        #endregion

        #region Utility Functions

        protected virtual void RemoveEnemyFromWorld()
        {
            QueueFree();
        }

        protected void MoveToTowardsTarget(Vector2 targetPosition) =>
            MoveToTowardsTarget(targetPosition, movementSpeed);

        protected void MoveToTowardsTarget(Vector2 targetPosition, float speed)
        {
            Vector2 directionVector = (targetPosition - GetGlobalPosition()).Normalized();
            MoveAndSlide(directionVector * speed);
        }

        protected void LookAtTarget(Vector2 target) => LookAt(target);

        protected Vector2 GetNewPositionForIdling() => VectorHelpers.Random2D() * explorationRadius;

        protected void HandleHealthZero()
        {
            SetEnemyState(EnemyState.Dead);
            _currentSoulCount = Mathf.FloorToInt((float)GD.RandRange(minSoulsAmount, maxSoulsAmount));
        }

        protected void SpawnSoul()
        {
            SoulsController soulsControllerInstance = (SoulsController)soulsPrefab.Instance();
            GetParent().AddChild(soulsControllerInstance);

            soulsControllerInstance.SetSoulsColor(_enemyColor);
            Vector2 randomVectorPosition = ExtensionFunctions.VectorRandomUnit();
            randomVectorPosition *= rangeMultiplier;
            Vector2 soulsPosition = GetGlobalPosition() + randomVectorPosition;
            soulsControllerInstance.SetGlobalPosition(soulsPosition);
        }

        protected void SetEnemyState(EnemyState enemyState)
        {
            if (_enemyState == enemyState)
            {
                return;
            }

            _enemyState = enemyState;
        }

        #endregion
    }
}