using Godot;
using StormTime.Common;

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

        public override void _ExitTree()
        {
            _firstArmHealthSetter.healthChanged -= HandleFirstArmHealthChange;
            _secondArmHealthSetter.healthChanged -= HandleSecondArmHealthChange;
        }

        #region Utility Functions

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

        #endregion

        #region External Functions

        public ArmStatus GetArmStatus() => _armStatus;

        public void LaunchDualArmAttack()
        {

        }

        public void LaunchFirstArmAttack()
        {

        }

        public void LaunchSecondArmAttack()
        {

        }

        #endregion
    }
}