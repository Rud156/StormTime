using System;
using Godot;
using StormTime.Common;

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

        // General Stats
        [Export] public float idleSwitchTimer;
        [Export] public float frenzyAttackChancePercent;
        [Export] public float frenzyAttackHealthPercent;

        // Attack Timers
        [Export] public float innerCircleShotTimer;
        [Export] public int innerCircleShotCount;
        [Export] public float singleArmShotTimer;
        [Export] public int singleArmShotCount;
        [Export] public float dualArmShotTimer;
        [Export] public int dualArmShotCount;
        [Export] public float circleWorldFillTimer;
        [Export] public int circleWorldFillCount;
        [Export] public float frenzyCircleShotTimer;
        [Export] public int frenzyCircleShotCount;
        [Export] public float bounceCircleShotTimer;
        [Export] public int bounceCircleShotCount;

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

            InnerCircleShot,
            SingleArmShot,
            DualArmShot,
            CircleWorldFill,
            BounceCircleShot,
            FrenzySpinningShot,

            Dead
        }

        private BossState _currentBossState;
        private float _bossTimer;

        private struct BossHealthBodyStatus
        {
            public bool doubleArmAvailable;
            public bool singleArmAvailable;
        }

        private BossHealthBodyStatus _bossHealthBodyStatus;
        private HealthSetter _bossTotalHealthSetter;

        public override void _Ready()
        {
            _bodyController = GetNode<BossBodyController>(bodyNodePath);
            _bodyController.bodyStatusChanged += HandleBodyStatusChanged;

            _leftArmController = GetNode<BossArmController>(leftArmNodePath);
            _rightArmController = GetNode<BossArmController>(rightArmNodePath);
            _topArmController = GetNode<BossArmController>(topArmNodePath);
            _bottomArmController = GetNode<BossArmController>(bottomArmNodePath);

            _leftArmController.armStatusChanged += HandleLeftArmStatusChanged;
            _rightArmController.armStatusChanged += HandleRightArmStatusChanged;
            _topArmController.armStatusChanged += HandleTopArmStatusChanged;
            _bottomArmController.armStatusChanged += HandleBottomArmStatusChanged;

            SetBossMaxHealth();
            ComputeTotalBossHealth();

            _bossHealthBodyStatus = new BossHealthBodyStatus();
            SetBossHealthBodyStatus();

            SetBossState(BossState.Idle);
        }

        public override void _ExitTree()
        {
            _bodyController.bodyStatusChanged -= HandleBodyStatusChanged;

            _leftArmController.armStatusChanged -= HandleLeftArmStatusChanged;
            _rightArmController.armStatusChanged -= HandleRightArmStatusChanged;
            _topArmController.armStatusChanged -= HandleTopArmStatusChanged;
            _bottomArmController.armStatusChanged -= HandleBottomArmStatusChanged;
        }

        public override void _Process(float delta)
        {
            switch (_currentBossState)
            {
                case BossState.Idle:
                    UpdateIdleState(delta);
                    break;

                case BossState.InnerCircleShot:
                    UpdateInnerCircleShot(delta);
                    break;

                case BossState.SingleArmShot:
                    UpdateSingleArmShot(delta);
                    break;

                case BossState.DualArmShot:
                    UpdateDualArmShot(delta);
                    break;

                case BossState.CircleWorldFill:
                    UpdateCircleWorldFill(delta);
                    break;

                case BossState.FrenzySpinningShot:
                    UpdateFrenzyCircleShot(delta);
                    break;

                case BossState.BounceCircleShot:
                    UpdateBounceCircleShot(delta);
                    break;

                case BossState.Dead:
                    UpdateDeadState(delta);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region State Updates

        private void UpdateIdleState(float delta)
        {
            _bossTimer -= delta;
            if (_bossTimer <= 0)
            {
                SetBossState(GetRandomAttack());
            }
        }

        private void UpdateInnerCircleShot(float delta)
        {

        }

        private void UpdateSingleArmShot(float delta)
        {

        }

        private void UpdateDualArmShot(float delta)
        {

        }

        private void UpdateCircleWorldFill(float delta)
        {

        }

        private void UpdateFrenzyCircleShot(float delta)
        {

        }

        private void UpdateBounceCircleShot(float delta)
        {

        }

        private void UpdateDeadState(float delta)
        {

        }

        #endregion

        #region Utility Functions

        private void HandleLeftArmStatusChanged(BossArmController.ArmStatus armStatus)
        {
            ComputeTotalBossHealth();
            ComputeArmExistenceStatus();
        }

        private void HandleRightArmStatusChanged(BossArmController.ArmStatus armStatus)
        {
            ComputeTotalBossHealth();
            ComputeArmExistenceStatus();
        }

        private void HandleTopArmStatusChanged(BossArmController.ArmStatus armStatus)
        {
            ComputeTotalBossHealth();
            ComputeArmExistenceStatus();
        }

        private void HandleBottomArmStatusChanged(BossArmController.ArmStatus armStatus)
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
            if (randomNumber >= 1)
            {
                randomNumber -= 0.01f; // This is done so as to not select the last Enum (Dead State)
            }

            BossState bossState = (BossState)(Mathf.FloorToInt(randomNumber * 5) + 1); // This removes the first Enum (Idle State)

            // This removes double arm attack in case it is selected and 
            // the boss does not have any double arms left
            while (bossState == BossState.DualArmShot && !_bossHealthBodyStatus.doubleArmAvailable)
            {
                randomNumber = (float)GD.Randf();
                if (randomNumber >= 1)
                {
                    randomNumber -= 0.01f;
                }
                bossState = (BossState)(Mathf.FloorToInt(randomNumber * 5) + 1);
            }

            // This removes single arm attack in case it is selected and 
            // the boss does not have any arms left
            while (bossState == BossState.SingleArmShot && !_bossHealthBodyStatus.singleArmAvailable)
            {
                randomNumber = (float)GD.Randf();
                if (randomNumber >= 1)
                {
                    randomNumber -= 0.01f;
                }
                bossState = (BossState)(Mathf.FloorToInt(randomNumber * 5) + 1);
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

                case BossState.InnerCircleShot:
                    _bossTimer = innerCircleShotTimer;
                    break;

                case BossState.SingleArmShot:
                    _bossTimer = singleArmShotTimer;
                    break;

                case BossState.DualArmShot:
                    _bossTimer = dualArmShotTimer;
                    break;

                case BossState.CircleWorldFill:
                    _bossTimer = circleWorldFillTimer;
                    break;

                case BossState.BounceCircleShot:
                    _bossTimer = bounceCircleShotTimer;
                    break;

                case BossState.FrenzySpinningShot:
                    _bossTimer = frenzyCircleShotTimer;
                    break;

                case BossState.Dead:
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