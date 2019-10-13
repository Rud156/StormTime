using Godot;
using StormTime.Common;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class BossBodyController : Node2D
    {
        // Body Health Controller
        [Export] public NodePath bossBodyNodePath;

        // Attack Points
        [Export] public NodePath bodyAttackNodePath;

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

        public override void _ExitTree() => _bossBodyHealthSetter.healthChanged -= HandleBodyHealthChanged;

        #region External Functions

        // Event Function from Bullet Collision
        public void BulletCollisionNotification(object bullet, bool isFreezingBullet)
        {
            bool isPLayerBullet = !(bullet is EnemyBullet);

            if (isPLayerBullet)
            {
                float damageAmount = ((Bullet)bullet).GetBulletDamage();
                _bossBodyHealthSetter.SubtractHealth(damageAmount);
            }
        }

        public BodyStatus GetBodyStatus() => _bodyStatus;

        #endregion

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
    }
}