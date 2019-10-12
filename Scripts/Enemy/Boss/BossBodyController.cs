using Godot;
using StormTime.Common;

namespace StormTime.Enemy.Boss
{
    public class BossBodyController : Node2D
    {
        // Body Health Controller
        [Export] public NodePath bossBodyNodePath;

        // Attack Points
        [Export] public NodePath firstAttackNodePath;
        [Export] public NodePath secondAttackNodePath;
        [Export] public NodePath thirdAttackNodePath;

        public delegate void BodyStatusChanged(BodyStatus bodyStatus);

        public BodyStatusChanged bodyStatusChanged;

        private HealthSetter _bossBodyHealthSetter;

        public struct BodyStatus
        {
            public bool bodyDestroyed;
            public float bodyHealth;
            public float bodyMaxHealth;
        }

        private BodyStatus _bodyStatus;

        public override void _Ready()
        {
            _bodyStatus = new BodyStatus()
            {
                bodyDestroyed = false,
                bodyHealth = 1
            };

            _bossBodyHealthSetter = GetNode<HealthSetter>(bossBodyNodePath);
            _bossBodyHealthSetter.healthChanged += HandleBodyHealthChanged;
        }

        public override void _ExitTree()
        {
            _bossBodyHealthSetter.healthChanged -= HandleBodyHealthChanged;
        }

        #region Utility Functions

        private void HandleBodyHealthChanged(float currentHealth, float maxHealth)
        {
            _bodyStatus.bodyHealth = currentHealth;
            _bodyStatus.bodyMaxHealth = maxHealth;

            if (currentHealth <= 0)
            {
                _bodyStatus.bodyDestroyed = true;
            }

            NotifyBodyHealthChanged();
        }

        private void NotifyBodyHealthChanged() => bodyStatusChanged?.Invoke(_bodyStatus);

        #endregion

        #region External Functions

        public BodyStatus GetBodyStatus() => _bodyStatus;

        #endregion
    }
}