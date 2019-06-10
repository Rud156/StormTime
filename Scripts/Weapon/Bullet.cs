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

        public override void _Ready()
        {
            _currentBulletTrailTimeLeft = bulletTrailTimer;
            _currentDamageAmount = bulletDamageAmount;
        }

        public override void _Process(float delta)
        {
            _currentBulletTrailTimeLeft -= delta;
            if (_currentBulletTrailTimeLeft <= 0)
            {
                SpawnBulletTrail();
                _currentBulletTrailTimeLeft = bulletTrailTimer;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            KinematicCollision2D collision = MoveAndCollide(_launchVelocity * delta);
            if (collision != null || _currentBulletTimeLeft <= 0)
            {
                if (collision != null)
                {
                    SpawnBulletExplosion();
                    NotifyCollider(collision.Collider);
                }

                DestroyBullet();
            }

            _currentBulletTimeLeft -= delta;
        }

        #region External Functions

        public void LaunchBullet(Vector2 forwardVectorNormalized)
        {
            _launchVelocity = forwardVectorNormalized * bulletSpeed;
            _currentBulletTimeLeft = bulletLifeTime;
        }

        public void LaunchBullet(Vector2 forwardVectorNormalized, float damageAmount)
        {
            _currentDamageAmount = damageAmount;
            LaunchBullet(forwardVectorNormalized);
        }

        public float GetBulletDamage() => _currentDamageAmount;

        #endregion

        #region Utility Functions

        private void DestroyBullet() => GetParent().RemoveChild(this);

        #endregion

        #region Events

        private void NotifyCollider(Godot.Object collider) =>
            collider.CallDeferred("BulletCollisionNotification", !(this is EnemyBullet));

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
