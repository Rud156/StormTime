using System.Diagnostics;
using Godot;

namespace StormTime.Weapon
{
    public class Bullet : KinematicBody2D
    {

        [Export] public PackedScene bulletTrailPrefab;
        [Export] public PackedScene bulletExplosionPrefab;
        [Export] public float bulletSpeed;
        [Export] public float bulletLifeTime;
        [Export] public float bulletTrailTimer;
        [Export] public float bulletDamageAmount;

        private Vector2 _launchVelocity;
        private float _currentBulletTimeLeft;
        private float _currentBulletTrailTimeLeft;

        private float _currentDamageAmount;
        private bool _destroyOnEnemyCollision;
        private bool _isStaticBullet;

        public override void _Ready()
        {
            _currentBulletTrailTimeLeft = bulletTrailTimer;
            _currentDamageAmount = bulletDamageAmount;

            _isStaticBullet = false;
        }

        public override void _Process(float delta)
        {
            if (_isStaticBullet)
            {
                return;
            }

            _currentBulletTrailTimeLeft -= delta;
            if (_currentBulletTrailTimeLeft <= 0)
            {
                SpawnBulletTrail();
                _currentBulletTrailTimeLeft = bulletTrailTimer;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_isStaticBullet)
            {
                return;
            }

            KinematicCollision2D collision = MoveAndCollide(_launchVelocity * delta);
            if (collision != null || _currentBulletTimeLeft <= 0)
            {
                if (collision != null)
                {
                    SpawnBulletExplosion();
                    NotifyCollider(collision.Collider);
                }

                if (!_destroyOnEnemyCollision && !(collision?.Collider is Enemy.Individuals.Enemy))
                {
                    DestroyBullet();
                }
                else if (_destroyOnEnemyCollision || _currentBulletTimeLeft <= 0)
                {
                    DestroyBullet();
                }
            }

            _currentBulletTimeLeft -= delta;
        }

        #region External Functions

        public void LaunchBullet(Vector2 forwardVectorNormalized, bool destroyOnEnemyCollision = true)
        {
            _launchVelocity = forwardVectorNormalized * bulletSpeed;
            _currentBulletTimeLeft = bulletLifeTime;

            _destroyOnEnemyCollision = destroyOnEnemyCollision;
        }

        public void LaunchBullet(Vector2 forwardVectorNormalized, float damageAmount, bool destroyOnEnemyCollision = true)
        {
            _currentDamageAmount = damageAmount;
            LaunchBullet(forwardVectorNormalized, destroyOnEnemyCollision);
        }

        public float GetBulletDamage() => _currentDamageAmount;

        public void SetBulletDamage(float damageAmount) => _currentDamageAmount = damageAmount;

        public void SetAsStaticBullet() => _isStaticBullet = true;

        public void SetAsDynamicBullet() => _isStaticBullet = false;

        #endregion

        #region Utility Functions

        private void DestroyBullet() => GetParent().RemoveChild(this);

        #endregion

        #region Events

        private void NotifyCollider(Object collider) =>
            collider.CallDeferred("BulletCollisionNotification", this);

        #endregion

        #region Particle Effects

        private void SpawnBulletExplosion()
        {
            Node2D bulletExplosionInstance = (Node2D)bulletExplosionPrefab.Instance();
            GetParent().AddChild(bulletExplosionInstance);

            bulletExplosionInstance.SetGlobalPosition(GetGlobalPosition());
        }

        private void SpawnBulletTrail()
        {
            Node2D bulletTrailInstance = (Node2D)bulletTrailPrefab.Instance();
            GetParent().AddChild(bulletTrailInstance);

            bulletTrailInstance.SetGlobalPosition(GetGlobalPosition());
        }

        #endregion
    }
}
