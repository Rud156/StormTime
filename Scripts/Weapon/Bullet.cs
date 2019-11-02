using Godot;

namespace StormTime.Weapon
{
    public class Bullet : RigidBody2D
    {
        [Export] public PackedScene bulletTrailPrefab;
        [Export] public PackedScene bulletExplosionPrefab;
        [Export] public float bulletSpeed;
        [Export] public float bulletLifeTime;
        [Export] public float bulletTrailTimer;
        [Export] public float bulletDamageAmount;
        [Export] public bool bulletHasTrail;
        [Export] public bool bulletHasExplosion;

        protected Vector2 _launchVelocity;
        protected float _currentBulletTimeLeft;
        protected float _currentBulletTrailTimeLeft;

        protected float _currentDamageAmount;
        protected bool _isFreezingBullet;

        protected Godot.Collections.Array _collidingBodies;

        public override void _Ready()
        {
            _currentBulletTrailTimeLeft = bulletTrailTimer;
            _currentDamageAmount = bulletDamageAmount;
        }

        public override void _Process(float delta) => HandleTrailSpawnAndUpdate(delta);

        public override void _PhysicsProcess(float delta)
        {
            _collidingBodies = GetCollidingBodies();
            if (_collidingBodies.Count != 0 || _currentBulletTimeLeft <= 0)
            {
                if (_collidingBodies.Count != 0)
                {
                    SpawnBulletExplosion();
                    NotifyCollider((Object)_collidingBodies[0]);
                }

                RemoveBulletFromTree();
            }

            _currentBulletTimeLeft -= delta;
        }

        #region External Functions

        public virtual void LaunchBullet(Vector2 forwardVectorNormalized)
        {
            _launchVelocity = forwardVectorNormalized * bulletSpeed;
            _currentBulletTimeLeft = bulletLifeTime;

            SetLinearVelocity(_launchVelocity);
        }

        public void ForceDestroyBullet()
        {
            SpawnBulletExplosion();
            RemoveBulletFromTree();
        }

        public float GetBulletDamage() => _currentDamageAmount;

        public void SetBulletDamage(float damageAmount) => _currentDamageAmount = damageAmount;

        public void SetFreezingBulletState(bool isFreezingBullet) => _isFreezingBullet = isFreezingBullet;

        #endregion

        #region Utility Functions

        protected void HandleTrailSpawnAndUpdate(float delta)
        {
            _currentBulletTrailTimeLeft -= delta;
            if (_currentBulletTrailTimeLeft <= 0)
            {
                SpawnBulletTrail();
                _currentBulletTrailTimeLeft = bulletTrailTimer;
            }
        }

        protected virtual void RemoveBulletFromTree() => QueueFree();

        #endregion

        #region Events

        protected void NotifyCollider(Object collider) =>
            collider.CallDeferred("BulletCollisionNotification", this, _isFreezingBullet);

        #endregion

        #region Particle Effects

        protected virtual void SpawnBulletExplosion()
        {
            if (!bulletHasExplosion)
            {
                return;
            }

            Node2D bulletExplosionInstance = (Node2D)bulletExplosionPrefab.Instance();
            GetParent().AddChild(bulletExplosionInstance);

            bulletExplosionInstance.SetGlobalPosition(GetGlobalPosition());
        }

        protected virtual void SpawnBulletTrail()
        {
            if (!bulletHasTrail)
            {
                return;
            }

            Node2D bulletTrailInstance = (Node2D)bulletTrailPrefab.Instance();
            GetParent().AddChild(bulletTrailInstance);

            bulletTrailInstance.SetGlobalPosition(GetGlobalPosition());
        }

        #endregion
    }
}
