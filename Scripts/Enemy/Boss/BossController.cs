using System;
using System.Collections.Generic;
using Godot;
using StormTime.Common;
using StormTime.Utils;

namespace StormTime.Enemy.Boss
{
    public class BossController : Node2D
    {
        // Body Parts
        [Export] public NodePath bodyNodePath;
        [Export] public NodePath leftArmNodePath;
        [Export] public NodePath rightArmNodePath;
        [Export] public NodePath topArmNodePath;
        [Export] public NodePath bottomArmNodePath;
        [Export] public NodePath bossTotalHealthSetter;

        // General Stats
        [Export] public float idleSwitchTimer;
        [Export] public float frenzyAttackChancePercent;
        [Export] public float frenzyAttackHealthPercent;
        [Export] public NodePath singleArmAttackNodePath;
        [Export] public NodePath dualArmAttackNodePath;
        [Export] public NodePath frenzyAttackNodePath;
        [Export] public Godot.Collections.Array<NodePath> bossAttacks;

        // Attack Selection Percentages
        [Export] public float minSingleArmAttackPercent;
        [Export] public float maxSingleArmAttackPercent;
        [Export] public float minDualArmAttackPercent;
        [Export] public float maxDualArmAttackPercent;
        [Export] public float minAbilityAttackPercent;
        [Export] public float maxAbilityArmAttackPercent;

        // Boss Movement
        [Export] public float movementRadius;
        [Export] public float movementSpeed;
        [Export] public float movementTimer;
        [Export] public float movementReachedDistance;

        public delegate void BossDead();
        public BossDead onBossDead;

        private BossBodyController _bodyController;
        private BossArmController _leftArmController;
        private BossArmController _rightArmController;
        private BossArmController _topArmController;
        private BossArmController _bottomArmController;

        private enum BossState
        {
            Idle,

            SingleArmShot,
            DualArmShot,
            AbilityAttack,
            FrenzySpinningShot,

            Dead
        }

        private BossState _currentBossState;
        private float _bossTimer;
        private int _abilityAttackIndex;

        private SingleArmAttack _singleArmAttack;
        private DoubleArmAttack _dualArmAttack;
        private FrenzySpinningShot _frenzySpinningShotAttack;
        private List<BossBaseAttack> _bossAttacks;

        private struct BossHealthBodyStatus
        {
            public bool doubleArmAvailable;
            public bool singleArmAvailable;
        }

        private BossHealthBodyStatus _bossHealthBodyStatus;
        private HealthSetter _bossTotalHealthSetter;

        private float _currentMovementTimer;
        private bool _bossReachedDestination;
        private Vector2 _targetMovementPosition;

        public override void _Ready()
        {
            _bodyController = GetNode<BossBodyController>(bodyNodePath);
            _bodyController.bodyStatusChanged += HandleBodyStatusChanged;

            _leftArmController = GetNode<BossArmController>(leftArmNodePath);
            _rightArmController = GetNode<BossArmController>(rightArmNodePath);
            _topArmController = GetNode<BossArmController>(topArmNodePath);
            _bottomArmController = GetNode<BossArmController>(bottomArmNodePath);

            _bossTotalHealthSetter = GetNode<HealthSetter>(bossTotalHealthSetter);

            _leftArmController.armStatusChanged += HandleArmStatusChanged;
            _rightArmController.armStatusChanged += HandleArmStatusChanged;
            _topArmController.armStatusChanged += HandleArmStatusChanged;
            _bottomArmController.armStatusChanged += HandleArmStatusChanged;

            _frenzySpinningShotAttack = GetNode<FrenzySpinningShot>(frenzyAttackNodePath);
            _singleArmAttack = GetNode<SingleArmAttack>(singleArmAttackNodePath);
            _dualArmAttack = GetNode<DoubleArmAttack>(dualArmAttackNodePath);
            _bossAttacks = new List<BossBaseAttack>();
            for (int i = 0; i < bossAttacks.Count; i++)
            {
                _bossAttacks.Add(GetNode<BossBaseAttack>(bossAttacks[i]));
            }

            GD.Print("Setting Boss Total Health");
            SetBossMaxHealth();
            ComputeTotalBossHealth();

            _bossHealthBodyStatus = new BossHealthBodyStatus();
            SetBossHealthBodyStatus();

            SetBossState(BossState.Idle);
        }

        public override void _ExitTree()
        {
            _bodyController.bodyStatusChanged -= HandleBodyStatusChanged;

            _leftArmController.armStatusChanged -= HandleArmStatusChanged;
            _rightArmController.armStatusChanged -= HandleArmStatusChanged;
            _topArmController.armStatusChanged -= HandleArmStatusChanged;
            _bottomArmController.armStatusChanged -= HandleArmStatusChanged;
        }

        public override void _Process(float delta)
        {
            UpdateBossMovement(delta);

            switch (_currentBossState)
            {
                case BossState.Idle:
                    UpdateIdleState(delta);
                    break;

                case BossState.SingleArmShot:
                    UpdateSingleArmShot(delta);
                    break;

                case BossState.DualArmShot:
                    UpdateDualArmShot(delta);
                    break;

                case BossState.AbilityAttack:
                    UpdateAbilityAttack(delta);
                    break;

                case BossState.FrenzySpinningShot:
                    UpdateFrenzySpinningShot(delta);
                    break;

                case BossState.Dead:
                    UpdateDeadState(delta);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_currentBossState), _currentBossState, null);
            }
        }

        #region State Updates

        private void UpdateBossMovement(float delta)
        {
            if (_bossReachedDestination)
            {
                _currentMovementTimer -= delta;
                if (_currentMovementTimer <= 0)
                {
                    _targetMovementPosition = VectorHelpers.Random2D() * movementRadius;
                    _bossReachedDestination = false;
                    _currentMovementTimer = movementTimer;
                }
            }
            else
            {
                Vector2 finalPosition = VectorHelpers.MoveTowards(GetGlobalPosition(), _targetMovementPosition, movementSpeed * delta);
                SetGlobalPosition(finalPosition);

                if (_targetMovementPosition.DistanceSquaredTo(finalPosition) <= movementReachedDistance)
                {
                    _bossReachedDestination = true;
                }
            }
        }

        private void UpdateIdleState(float delta)
        {
            _bossTimer -= delta;
            if (_bossTimer <= 0)
            {
                SetBossState(GetRandomAttack());
            }

            // TODO: Remove this after testing all attacks

            // // Switch To Idle State
            // if (Input.IsActionJustPressed(SceneControls.Testing_1))
            // {
            //     SetBossState(BossState.Idle);
            // }
            // // Single Arm Attack
            // else if (Input.IsActionJustPressed(SceneControls.Testing_2))
            // {
            //     SetBossState(BossState.SingleArmShot);
            // }
            // // Double Arm Attack
            // else if (Input.IsActionJustPressed(SceneControls.Testing_3))
            // {
            //     SetBossState(BossState.DualArmShot);
            // }
            // // Inner Circle Shot
            // else if (Input.IsActionJustPressed(SceneControls.Testing_4))
            // {
            //     SetBossState(BossState.AbilityAttack);
            //     _abilityAttackIndex = 2;
            // }
            // // Bounce Circle Shot
            // else if (Input.IsActionJustPressed(SceneControls.Testing_5))
            // {
            //     SetBossState(BossState.AbilityAttack);
            //     _abilityAttackIndex = 0;
            // }
            // // Circle World Fill
            // else if (Input.IsActionJustPressed(SceneControls.Testing_6))
            // {
            //     SetBossState(BossState.AbilityAttack);
            //     _abilityAttackIndex = 1;
            // }
            // // Frenzy Attack Shot
            // else if (Input.IsActionJustPressed(SceneControls.Testing_7))
            // {
            //     SetBossState(BossState.FrenzySpinningShot);
            // }
        }

        private void UpdateSingleArmShot(float delta)
        {
            bool singleArmAttackComplete = _singleArmAttack.UpdateAttack(delta);
            if (singleArmAttackComplete)
            {
                SetBossState(BossState.Idle);
            }
        }

        private void UpdateDualArmShot(float delta)
        {
            bool dualArmAttackComplete = _dualArmAttack.UpdateAttack(delta);
            if (dualArmAttackComplete)
            {
                SetBossState(BossState.Idle);
            }
        }

        private void UpdateFrenzySpinningShot(float delta)
        {
            bool frenzyAttackComplete = _frenzySpinningShotAttack.UpdateAttack(delta);
            if (frenzyAttackComplete)
            {
                SetBossState(BossState.Idle);
            }
        }

        private void UpdateAbilityAttack(float delta)
        {
            bool abilityAttackComplete = _bossAttacks[_abilityAttackIndex].UpdateAttack(delta);
            if (abilityAttackComplete)
            {
                SetBossState(BossState.Idle);
            }
        }

        private void UpdateDeadState(float delta)
        {
            // TODO: Do something here...
        }

        #endregion

        #region Utility Functions

        private void HandleArmStatusChanged(BossArmController.ArmStatus armStatus)
        {
            ComputeTotalBossHealth();
            ComputeArmExistenceStatus();
        }

        private void HandleBodyStatusChanged(BossBodyController.BodyStatus bodyStatus)
        {
            ComputeTotalBossHealth();

            // If the body is not there, there is nothing the Boss can do
            if (bodyStatus.bodyDestroyed)
            {
                DestroyBoss();
            }
        }

        private void DestroyBoss()
        {
            // TODO: Also do something else like effects and other things

            onBossDead?.Invoke();
            SetBossState(BossState.Dead);
        }

        private void ComputeTotalBossHealth()
        {
            BossArmController.ArmStatus leftArmStatus = _leftArmController.GetArmStatus();
            BossArmController.ArmStatus rightArmStatus = _rightArmController.GetArmStatus();
            BossArmController.ArmStatus topArmStatus = _topArmController.GetArmStatus();
            BossArmController.ArmStatus bottomArmStatus = _bottomArmController.GetArmStatus();

            BossBodyController.BodyStatus bodyStatus = _bodyController.GetBodyStatus();

            float totalHealth = leftArmStatus.firstArmHealth + leftArmStatus.secondArmHealth;
            totalHealth += (rightArmStatus.firstArmHealth + rightArmStatus.secondArmHealth);
            totalHealth += (topArmStatus.firstArmHealth + topArmStatus.secondArmHealth);
            totalHealth += (bottomArmStatus.firstArmHealth + bottomArmStatus.secondArmHealth);
            totalHealth += bodyStatus.bodyHealth;

            GD.Print($"Total Health: {totalHealth}");

            _bossTotalHealthSetter.ForceSetCurrentHealth(totalHealth);
        }

        private void ComputeArmExistenceStatus()
        {
            BossArmController.ArmStatus leftArmStatus = _leftArmController.GetArmStatus();
            BossArmController.ArmStatus rightArmStatus = _rightArmController.GetArmStatus();
            BossArmController.ArmStatus topArmStatus = _topArmController.GetArmStatus();
            BossArmController.ArmStatus bottomArmStatus = _bottomArmController.GetArmStatus();

            bool hasDoubleArm = (leftArmStatus.firstArmAlive && leftArmStatus.secondArmAlive) ||
                                (rightArmStatus.firstArmAlive && rightArmStatus.secondArmAlive) ||
                                (topArmStatus.firstArmAlive && topArmStatus.secondArmAlive) ||
                                (bottomArmStatus.firstArmAlive && bottomArmStatus.secondArmAlive);

            bool hasSingleArm = hasDoubleArm ||
                                (leftArmStatus.firstArmAlive || leftArmStatus.secondArmAlive) ||
                                (rightArmStatus.firstArmAlive || rightArmStatus.secondArmAlive) ||
                                (topArmStatus.firstArmAlive || topArmStatus.secondArmAlive) ||
                                (bottomArmStatus.firstArmAlive || bottomArmStatus.secondArmAlive);

            SetBossHealthBodyStatus(hasDoubleArm, hasSingleArm);
        }

        private void SetBossMaxHealth()
        {
            BossArmController.ArmStatus leftArmStatus = _leftArmController.GetArmStatus();
            BossArmController.ArmStatus rightArmStatus = _rightArmController.GetArmStatus();
            BossArmController.ArmStatus topArmStatus = _topArmController.GetArmStatus();
            BossArmController.ArmStatus bottomArmStatus = _bottomArmController.GetArmStatus();

            BossBodyController.BodyStatus bodyStatus = _bodyController.GetBodyStatus();

            float totalMaxHealth = leftArmStatus.firstArmMaxHealth + leftArmStatus.secondArmMaxHealth;
            totalMaxHealth += (rightArmStatus.firstArmMaxHealth + rightArmStatus.secondArmMaxHealth);
            totalMaxHealth += (topArmStatus.firstArmMaxHealth + topArmStatus.secondArmMaxHealth);
            totalMaxHealth += (bottomArmStatus.firstArmMaxHealth + bottomArmStatus.secondArmMaxHealth);
            totalMaxHealth += bodyStatus.bodyMaxHealth;

            GD.Print($"Max Boss Health: {totalMaxHealth}");

            _bossTotalHealthSetter.SetMaxHealth(totalMaxHealth);

        }

        private BossState GetRandomAttack()
        {
            if (_currentBossState == BossState.Dead)
            {
                return BossState.Dead;
            }

            float frenzyAttackChance = (float)GD.Randf();
            float currentHealth = _bossTotalHealthSetter.GetCurrentHealth();

            if (frenzyAttackChance <= frenzyAttackChancePercent &&
                currentHealth <= frenzyAttackHealthPercent)
            {
                return BossState.FrenzySpinningShot;
            }

            float randomNumber = (float)GD.Randf();
            BossState bossState = BossState.FrenzySpinningShot;

            if (randomNumber > minSingleArmAttackPercent && randomNumber < maxSingleArmAttackPercent)
            {
                bossState = BossState.SingleArmShot;
            }
            else if (randomNumber > minDualArmAttackPercent && randomNumber < maxDualArmAttackPercent)
            {
                bossState = BossState.DualArmShot;
            }
            else if (randomNumber > minAbilityAttackPercent && randomNumber < maxAbilityArmAttackPercent)
            {
                bossState = BossState.AbilityAttack;
            }

            if (bossState == BossState.SingleArmShot && !_bossHealthBodyStatus.singleArmAvailable)
            {
                bossState = BossState.AbilityAttack;
            }
            else if (bossState == BossState.DualArmShot && !_bossHealthBodyStatus.doubleArmAvailable)
            {
                bossState = BossState.AbilityAttack;
            }

            // This is very important.
            // Probably should not be hidden like this
            if (bossState == BossState.AbilityAttack)
            {
                _abilityAttackIndex = (int)(GD.Randi() % _bossAttacks.Count);
                _abilityAttackIndex = Mathf.Abs(_abilityAttackIndex); // This is added so as to prevent an overflow
            }

            return bossState;
        }

        private void SetBossState(BossState bossState)
        {
            if (bossState == _currentBossState)
            {
                return;
            }

            _currentBossState = bossState;
            switch (_currentBossState)
            {
                case BossState.Idle:
                    _bossTimer = idleSwitchTimer;
                    break;

                case BossState.SingleArmShot:
                case BossState.DualArmShot:
                case BossState.FrenzySpinningShot:
                case BossState.AbilityAttack:
                    // Don't do anything here as the base attack handles the return state
                    break;

                case BossState.Dead:
                    // TODO: Add Something here probably
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        private void SetBossHealthBodyStatus(bool doubleArmAvailable = true, bool singleArmAvailable = true)
        {
            _bossHealthBodyStatus.singleArmAvailable = singleArmAvailable;
            _bossHealthBodyStatus.doubleArmAvailable = doubleArmAvailable;
        }

        #endregion
    }
}