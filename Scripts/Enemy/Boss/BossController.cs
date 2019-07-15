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
        private int _bossStateCounter;

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

        }

        private void HandleRightArmStatusChanged(BossArmController.ArmStatus armStatus)
        {

        }

        private void HandleTopArmStatusChanged(BossArmController.ArmStatus armStatus)
        {

        }

        private void HandleBottomArmStatusChanged(BossArmController.ArmStatus armStatus)
        {

        }

        private void HandleBodyStatusChanged(BossBodyController.BodyStatus bodyStatus)
        {

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
                randomNumber -= 0.01f;
            }

            return (BossState)(Mathf.FloorToInt(randomNumber * 5) + 1);
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

        #endregion
    }
}