using Godot;
using System;
using StormTime.Utils;

namespace StormTime.Weapon
{
    public class Bullet : KinematicBody2D
    {
        private static readonly PackedScene BulletExplosion =
            ResourceLoader.Load<PackedScene>(GameConstants.BulletExplosionPrefab);

        private static readonly PackedScene BulletTrail =
            ResourceLoader.Load<PackedScene>(GameConstants.BulletTrailPrefab);

        [Export] public float bulletSpeed;
        [Export] public float bulletLifeTime;
        [Export] public float bulletTrailTimer;

        private Vector2 _launchVelocity;
        private float _currentBulletTimeLeft;
        private float _currentBulletTrailTimeLeft;

        public override void _Ready()
        {
            base._Ready();

            _currentBulletTrailTimeLeft = bulletTrailTimer;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

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

        public void LaunchBullet(Vector2 playerForwardVector)
        {
            _launchVelocity = playerForwardVector * bulletSpeed;
            _currentBulletTimeLeft = bulletLifeTime;
        }

        private void DestroyBullet() => GetParent().RemoveChild(this);

        private void SpawnBulletExplosion()
        {
            Node2D bulletExplosionInstance = (Node2D)BulletExplosion.Instance();
            bulletExplosionInstance.SetPosition(GetGlobalPosition());
            GetParent().AddChild(bulletExplosionInstance);
        }

        private void SpawnBulletTrail()
        {
            Node2D bulletTrailInstance = (Node2D) BulletTrail.Instance();
            bulletTrailInstance.SetPosition(GetGlobalPosition());
            GetParent().AddChild(bulletTrailInstance);
        }
    }
}