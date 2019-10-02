using System;
using System.Collections.Generic;
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
        [Export] public NodePath frenzyAttackNodePath;
        [Export] public Godot.Collections.Array<NodePath> bossAttacks;

        // Attack Timers
        [Export] public float singleArmShotTimer;
        [Export] public int singleArmShotCount;
        [Export] public float dualArmShotTimer;
        [Export] public int dualArmShotCount;

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

        private FrenzySpinningShot _frenzySpinningShotAttack;
        private List<BossBaseAttack> _bossAttacks;

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

            _frenzySpinningShotAttack = GetNode<FrenzySpinningShot>(frenzyAttackNodePath);
            _bossAttacks = new List<BossBaseAttack>();
            for (int i = 0; i < bossAttacks.Count; i++)
            {
                _bossAttacks.Add(GetNode<BossBaseAttack>(bossAttacks[i]));
            }

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

        private void UpdateSingleArmShot(float delta)
        {
            _bossTimer -= delta;
            if (_bossTimer <= 0)
            {
                SetBossState(GetRandomAttack());
            }
        }

        private void UpdateDualArmShot(float delta)
        {
            _bossTimer -= delta;
            if (_bossTimer <= 0)
            {
                SetBossState(GetRandomAttack());
            }
        }

        private void UpdateFrenzySpinningShot(float delta)
        {
            bool frenzyAttackUpdate = _frenzySpinningShotAttack.Update(delta);
            if (frenzyAttackUpdate)
            {
                SetBossState(GetRandomAttack());
            }
        }

        private void UpdateAbilityAttack(float delta)
        {
            bool abilityAttackUpdate = _bossAttacks[_abilityAttackIndex].Update(delta);
            if (abilityAttackUpdate)
            {
                SetBossState(GetRandomAttack());
            }
        }

        private void UpdateDeadState(float delta)
        {
            // TODO: Do something here...
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

            BossState bossState = (BossState)(Mathf.FloorToInt(randomNumber * 3) + 1); // This removes the first Enum (Idle State)

            // This removes double arm attack in case it is selected and 
            // the boss does not have any double arms left
            while (bossState == BossState.DualArmShot && !_bossHealthBodyStatus.doubleArmAvailable)
            {
                randomNumber = (float)GD.Randf();
                if (randomNumber >= 1)
                {
                    randomNumber -= 0.01f;
                }
                bossState = (BossState)(Mathf.FloorToInt(randomNumber * 3) + 1);
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
                bossState = (BossState)(Mathf.FloorToInt(randomNumber * 3) + 1);
            }

            if (bossState == BossState.AbilityAttack)
            {
                _abilityAttackIndex = ((int)GD.Randi() % _bossAttacks.Count) - 1; // Select a random ability in case ability attack is selected
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
                    _bossTimer = singleArmShotTimer;
                    break;

                case BossState.DualArmShot:
                    _bossTimer = dualArmShotTimer;
                    break;

                case BossState.FrenzySpinningShot:
                    // Don't do anything here as the base attack handles the return state
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