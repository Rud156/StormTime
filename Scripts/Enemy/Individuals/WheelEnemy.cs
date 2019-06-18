using Godot;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Enemy.Individuals
{
    public class WheelEnemy : Enemy
    {
        // Shooting Rotation
        [Export] public float rotationRate;
        [Export] public float lowHealthRotationRate;
        [Export] public float lowHealthActivatorRatio;

        // Shooting Info
        [Export] public float totalShotCount;

        // Enemy Bullet and Health
        [Export] public PackedScene enemyBulletPrefab;

        private float _shootingTimer;
        private float _timeBetweenShots;
        private float _currentRotationRate;

        public override void _Ready()
        {
            base._Ready();

            _enemyHealthSetter.healthChanged += SetHealthBasedRotation;
            SetHealthBasedRotation(_enemyHealthSetter.GetCurrentHealth(), _enemyHealthSetter.GetMaxHealth());
        }

        public override void _ExitTree() => _enemyHealthSetter.healthChanged -= SetHealthBasedRotation;

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);

            if (_shootingTimer > 0)
            {
                _shootingTimer -= delta;
            }
            else
            {
                _shootingTimer = _timeBetweenShots;
                ShootBullets();
            }

            RotateEnemy(delta);
        }

        protected override void LaunchAttack()
        {
            base.LaunchAttack();

            _shootingTimer = 0;
            _timeBetweenShots = attackTime / totalShotCount;
        }

        #region Enemy Attack Utilities

        private void RotateEnemy(float delta) =>
            _rotationNode.Rotate(Mathf.Deg2Rad(_currentRotationRate * delta));

        private void ShootBullets()
        {
            float startRotation = 45;
            float rotationIncrementAmount = 360.0f / _launchPoints.Count;

            foreach (Node2D launchPoint in _launchPoints)
            {
                EnemyBullet bulletInstance = (EnemyBullet)enemyBulletPrefab.Instance();
                GetParent().AddChild(bulletInstance);

                bulletInstance.SetBulletColor(_bulletColor);
                bulletInstance.SetGlobalPosition(launchPoint.GetGlobalPosition());

                float rotation = _rotationNode.GetRotationDegrees() + startRotation;
                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(rotation));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(rotation));
                Vector2 launchVector = new Vector2(xVelocity, yVelocity);
                bulletInstance.LaunchBullet(launchVector.Normalized());

                startRotation += rotationIncrementAmount;
            }
        }

        private void SetHealthBasedRotation(float currentHealth, float maxHealth)
        {
            float healthRatio = currentHealth / maxHealth;
            _currentRotationRate = healthRatio <= lowHealthActivatorRatio ?
                lowHealthRotationRate : rotationRate;
        }

        #endregion
    }
}