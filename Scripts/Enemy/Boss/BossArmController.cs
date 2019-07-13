using Godot;
using StormTime.Common;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class BossArmController : Node2D
    {
        // Arm Health Controllers
        [Export] public NodePath firstArmNodePath;
        [Export] public NodePath secondArmNodePath;

        // Attack Positions
        [Export] public NodePath firstArmAttackNodePath;
        [Export] public NodePath secondArmAttackNodePath;
        [Export] public NodePath dualArmAttackNodePath;

        // Arm Attacks
        [Export] public PackedScene simpleBulletPrefab;
        [Export] public PackedScene chargedBulletPrefab;
        [Export] public PackedScene chargingEffectPrefab;

        // Attack Information
        [Export] public float multiShotCircleAttackAngleDiff;
        [Export] public float timeBetweenSimpleAttacks;
        [Export] public float timeBetweenChargedAttacks;

        public delegate void ArmStatusChanged(ArmStatus armStatus);

        public ArmStatusChanged armStatusChanged;

        private HealthSetter _firstArmHealthSetter;
        private HealthSetter _secondArmHealthSetter;
        private Node2D _firstArmAttackPosition;
        private Node2D _secondArmAttackPosition;
        private Node2D _dualArmAttackPosition;

        public struct ArmStatus
        {
            public bool firstArmAlive;
            public bool secondArmAlive;

            public float firstArmHealth;
            public float secondArmHealth;
        }

        private ArmStatus _armStatus;

        private enum ArmAttackState
        {
            IdleState,
            DualArmAttack,
            FirstArmAttack,
            SecondArmAttack
        }

        private ArmAttackState _armAttackState;
        private float _attackTimer;

        private float _attackVariable_1; // Used for multiple things such as charge and rotation etc...
        private float _attackVariable_2; // Used for multiple things such as charge and rotation etc...
        private Object _attackIndicator;

        public override void _Ready()
        {
            _armStatus = new ArmStatus()
            {
                firstArmAlive = true,
                secondArmAlive = true,

                firstArmHealth = 1,
                secondArmHealth = 1
            };

            _firstArmHealthSetter = GetNode<HealthSetter>(firstArmNodePath);
            _secondArmHealthSetter = GetNode<HealthSetter>(secondArmNodePath);

            _firstArmAttackPosition = GetNode<Node2D>(firstArmAttackNodePath);
            _secondArmAttackPosition = GetNode<Node2D>(secondArmAttackNodePath);
            _dualArmAttackPosition = GetNode<Node2D>(dualArmAttackNodePath);

            _firstArmHealthSetter.healthChanged += HandleFirstArmHealthChange;
            _secondArmHealthSetter.healthChanged += HandleSecondArmHealthChange;
        }

        public override void _Process(float delta)
        {
            switch (_armAttackState)
            {
                case ArmAttackState.IdleState:
                    UpdateIdleArmState(delta);
                    break;

                case ArmAttackState.FirstArmAttack:
                    UpdateFirstArmAttack(delta);
                    break;

                case ArmAttackState.SecondArmAttack:
                    UpdateSecondArmAttack(delta);
                    break;

                case ArmAttackState.DualArmAttack:
                    UpdateDualArmAttack(delta);
                    break;
            }
        }

        public override void _ExitTree()
        {
            _firstArmHealthSetter.healthChanged -= HandleFirstArmHealthChange;
            _secondArmHealthSetter.healthChanged -= HandleSecondArmHealthChange;
        }

        #region State Updates

        private void UpdateIdleArmState(float delta)
        {
            // Don't do anything here. Might be used later on...
        }

        private void UpdateFirstArmAttack(float delta)
        {
            _attackTimer -= delta;
            if (_attackTimer <= 0)
            {
                SetArmAttackState(ArmAttackState.IdleState);
            }

            _attackVariable_2 -= delta;
            if (_attackVariable_2 <= 0)
            {
                _attackVariable_2 = timeBetweenSimpleAttacks;
                LaunchSingleArmAttack(_firstArmAttackPosition.GetGlobalPosition());
            }
        }

        private void UpdateSecondArmAttack(float delta)
        {
            _attackTimer -= delta;
            if (_attackTimer <= 0)
            {
                SetArmAttackState(ArmAttackState.IdleState);
            }

            _attackVariable_2 -= delta;
            if (_attackVariable_2 <= 0)
            {
                _attackVariable_2 = timeBetweenSimpleAttacks;
                LaunchSingleArmAttack(_secondArmAttackPosition.GetGlobalPosition());
            }
        }

        private void UpdateDualArmAttack(float delta)
        {
            _attackTimer -= delta;
            if (_attackTimer <= 0)
            {
                SetArmAttackState(ArmAttackState.IdleState);
            }

            _attackVariable_2 -= delta;
            if (_attackVariable_2 <= 0)
            {
                _attackVariable_2 = timeBetweenChargedAttacks;
                LaunchChargedAttack();
            }
        }

        #endregion

        #region External Functions

        public ArmStatus GetArmStatus() => _armStatus;

        public void LaunchDualArmAttack(float attackTimer)
        {
            _attackTimer = attackTimer;
            SetArmAttackState(ArmAttackState.DualArmAttack);
        }

        public void LaunchFirstArmAttack(float attackTimer)
        {
            _attackTimer = attackTimer;
            SetArmAttackState(ArmAttackState.FirstArmAttack);
        }

        public void LaunchSecondArmAttack(float attackTimer)
        {
            _attackTimer = attackTimer;
            SetArmAttackState(ArmAttackState.SecondArmAttack);
        }

        #endregion

        #region Utility Functions

        private void LaunchSingleArmAttack(Vector2 attackPosition)
        {
            float xVelocity = Mathf.Cos(Mathf.Deg2Rad(_attackVariable_1));
            float yVelocity = Mathf.Sin(Mathf.Deg2Rad(_attackVariable_1));
            Vector2 launchVelocity = new Vector2(xVelocity, yVelocity);

            BossBullet bulletInstance = (BossBullet)simpleBulletPrefab.Instance();
            GetParent().GetParent().AddChild(bulletInstance);

            bulletInstance.SetGlobalPosition(_firstArmAttackPosition.GetGlobalPosition());
            bulletInstance.LaunchBullet(launchVelocity);

            _attackVariable_1 += multiShotCircleAttackAngleDiff;
        }

        private void LaunchChargedAttack()
        {

        }

        private void HandleFirstArmHealthChange(float currentHealth, float maxHealth)
        {
            _armStatus.firstArmHealth = currentHealth;

            if (currentHealth <= 0)
            {
                _armStatus.firstArmAlive = false;
            }

            NotifyArmStatusChanged();
        }

        private void HandleSecondArmHealthChange(float currentHealth, float maxHealth)
        {
            _armStatus.secondArmHealth = currentHealth;

            if (currentHealth <= 0)
            {
                _armStatus.secondArmAlive = false;
            }

            NotifyArmStatusChanged();
        }

        private void NotifyArmStatusChanged() => armStatusChanged?.Invoke(_armStatus);

        private void SetArmAttackState(ArmAttackState armAttackState)
        {
            if (_armAttackState == armAttackState)
            {
                return;
            }

            _armAttackState = armAttackState;
        }

        #endregion
    }
}