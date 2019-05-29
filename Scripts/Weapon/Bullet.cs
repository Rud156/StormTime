using Godot;
using System;
using StormTime.Utils;

namespace StormTime.Weapon
{
    public class Bullet : KinematicBody2D
    {

        [Export] public PackedScene bulletTrailPrefab;
        [Export] public PackedScene bulletExplosionPrefab;
        [Export] public float bulletSpeed;
        [Export] public float bulletLifeTime;
        [Export] public float bulletTrailTimer;

        private Vector2 _launchVelocity;
        private float _currentBulletTimeLeft;
        private float _currentBulletTrailTimeLeft;

        public override void _Ready() => _currentBulletTrailTimeLeft = bulletTrailTimer;

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
                }

                DestroyBullet();
            }

            _currentBulletTimeLeft -= delta;
        }

        public void LaunchBullet(Vector2 forwardVectorNormalized)
        {
            _launchVelocity = forwardVectorNormalized * bulletSpeed;
            _currentBulletTimeLeft = bulletLifeTime;
        }

        private void DestroyBullet() => GetParent().RemoveChild(this);

        private void SpawnBulletExplosion()
        {
            Node2D bulletExplosionInstance = (Node2D)bulletExplosionPrefab.Instance();
            GetParent().AddChild(bulletExplosionInstance);

            bulletExplosionInstance.SetGlobalPosition(GetGlobalPosition());
        }

        protected virtual void SpawnBulletTrail()
        {
            Node2D bulletTrailInstance = (Node2D)bulletTrailPrefab.Instance();
            GetParent().AddChild(bulletTrailInstance);

            bulletTrailInstance.SetGlobalPosition(GetGlobalPosition());
        }
    }
}
