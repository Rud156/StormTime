using Godot;
using StormTime.Common;
using StormTime.Extensions;
using StormTime.Player.Data;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class BossArmController : Node2D
    {
        // Arm Controllers
        [Export] public NodePath firstArmNodePath;
        [Export] public NodePath secondArmNodePath;
        [Export] public NodePath firstArmSingleControllerNodePath;
        [Export] public NodePath secondArmSingleControllerNodePath;

        // Attack Positions
        [Export] public NodePath firstArmAttackNodePath;
        [Export] public NodePath secondArmAttackNodePath;
        [Export] public NodePath dualArmAttackNodePath;
        [Export] public NodePath bulletHolderNodePath;

        // Arm Attacks
        [Export] public PackedScene singleArmBulletPrefab;
        [Export] public PackedScene chargedBulletPrefab;
        [Export] public PackedScene chargingEffectPrefab;

        // Attack Information
        [Export] public float singeArmAttackAngleDiff;
        [Export] public float timeBetweenSingleArmAttacks;
        [Export] public float timeBetweenChargedAttacks;
        [Export] public float chargedAttackIncreaseRate;

        public delegate void ArmStatusChanged(ArmStatus armStatus);
        public ArmStatusChanged armStatusChanged;

        public delegate void ArmDestroyed();
        public ArmDestroyed armDestroyed;


        private HealthSetter _firstArmHealthSetter;
        private HealthSetter _secondArmHealthSetter;
        private Node2D _firstArmAttackPosition;
        private Node2D _secondArmAttackPosition;
        private Node2D _dualArmAttackPosition;

        // Bullets
        private Node2D _bulletHolder;
        private BossBullet _chargedBullet;
        private DestroyNodeForced _chargedEffectDestroy;

        // Arm Single Controllers
        private BossSingleArmController _firstArmSingleController;
        private BossSingleArmController _secondArmSingleController;

        public struct ArmStatus
        {
            public bool firstArmAlive;
            public bool secondArmAlive;

            public float firstArmHealth;
            public float secondArmHealth;

            public float firstArmMaxHealth;
            public float secondArmMaxHealth;
        }

        private ArmStatus _armStatus;

        private enum ArmAttackState
        {
            IdleState,
            DualArmAttack,
            FirstArmAttack,
            SecondArmAttack,
        }

        private ArmAttackState _armAttackState;
        private float _attackTimer;

        private float _attackVariable_1; // Used for multiple things such as charge and rotation etc...
        private float _attackVariable_2; // Used for multiple things such as charge and rotation etc...

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

            _firstArmSingleController = GetNode<BossSingleArmController>(firstArmSingleControllerNodePath);
            _secondArmSingleController = GetNode<BossSingleArmController>(secondArmSingleControllerNodePath);

            _firstArmAttackPosition = GetNode<Node2D>(firstArmAttackNodePath);
            _secondArmAttackPosition = GetNode<Node2D>(secondArmAttackNodePath);
            _dualArmAttackPosition = GetNode<Node2D>(dualArmAttackNodePath);

            _bulletHolder = GetNode<Node2D>(bulletHolderNodePath);

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
                _attackVariable_2 = timeBetweenSingleArmAttacks;
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
                _attackVariable_2 = timeBetweenSingleArmAttacks;
                LaunchSingleArmAttack(_secondArmAttackPosition.GetGlobalPosition());
            }
        }

        private void UpdateDualArmAttack(float delta)
        {
            _attackTimer -= delta;
            if (_attackTimer <= 0)
            {
                LaunchChargedAttack();
                SetArmAttackState(ArmAttackState.IdleState);
            }

            _attackVariable_2 -= delta;
            UpdateChargedAttack(delta);

            if (_attackVariable_2 <= 0)
            {
                _attackVariable_2 = timeBetweenChargedAttacks;
                LaunchChargedAttack();
                CreateChargedAttack();
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

            BossBullet bulletInstance = (BossBullet)singleArmBulletPrefab.Instance();
            _bulletHolder.AddChild(bulletInstance);

            bulletInstance.SetGlobalPosition(attackPosition);
            bulletInstance.LaunchBullet(launchVelocity);

            _attackVariable_1 += singeArmAttackAngleDiff;
            _attackVariable_1 = ExtensionFunctions.To360Angle(_attackVariable_1);
        }

        #region Charged Bullet

        private void CreateChargedAttack()
        {
            // Charged Bullet
            _chargedBullet = (BossBullet)chargedBulletPrefab.Instance();
            _dualArmAttackPosition.AddChild(_chargedBullet);
            _chargedBullet.SetMode(RigidBody2D.ModeEnum.Kinematic);
            _chargedBullet.SetGlobalPosition(_dualArmAttackPosition.GetGlobalPosition());

            // Charged Bullet Effect
            _chargedEffectDestroy = (DestroyNodeForced)chargingEffectPrefab.Instance();
            _dualArmAttackPosition.AddChild(_chargedEffectDestroy);
        }

        private void UpdateChargedAttack(float delta)
        {
            _attackVariable_1 += delta * chargedAttackIncreaseRate;
            _chargedBullet.SetGlobalScale(Vector2.One * _attackVariable_1);
        }

        private void LaunchChargedAttack()
        {
            if (_chargedBullet == null)
            {
                return;
            }

            Vector2 launchPosition = _dualArmAttackPosition.GetGlobalPosition();
            float launchAngle = Mathf.Atan2(
                launchPosition.x - PlayerVariables.LastPlayerPosition.x,
                launchPosition.y - PlayerVariables.LastPlayerPosition.y
            );

            float xVelocity = Mathf.Cos(launchAngle);
            float yVelocity = Mathf.Sin(launchAngle);
            Vector2 launchVelocity = new Vector2(xVelocity, yVelocity);

            _dualArmAttackPosition.RemoveChild(_chargedBullet);
            _bulletHolder.AddChild(_chargedBullet);

            _chargedBullet.SetMode(RigidBody2D.ModeEnum.Rigid);
            _chargedBullet.LaunchBullet(launchVelocity);

            _chargedEffectDestroy.DestroyNode();

            _attackVariable_1 = 0;
            _chargedEffectDestroy = null;
            _chargedBullet = null;
        }

        #endregion

        private void HandleFirstArmHealthChange(float currentHealth, float maxHealth)
        {
            _armStatus.firstArmHealth = currentHealth;
            _armStatus.firstArmMaxHealth = maxHealth;

            if (currentHealth <= 0)
            {
                _armStatus.firstArmAlive = false;

                _firstArmSingleController.DestroyArm();
                _firstArmHealthSetter.healthChanged -= HandleFirstArmHealthChange;
            }

            NotifyArmStatusChanged();
        }

        private void HandleSecondArmHealthChange(float currentHealth, float maxHealth)
        {
            _armStatus.secondArmHealth = currentHealth;
            _armStatus.secondArmMaxHealth = maxHealth;

            if (currentHealth <= 0)
            {
                _armStatus.secondArmAlive = false;

                _secondArmSingleController.DestroyArm();
                _secondArmHealthSetter.healthChanged -= HandleSecondArmHealthChange;
            }

            NotifyArmStatusChanged();
        }

        private void NotifyArmStatusChanged() => armStatusChanged?.Invoke(_armStatus);

        private void NotifyArmDestroyed() => armDestroyed?.Invoke();

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