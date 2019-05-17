using Godot;
using System;
using StormTime.Utils;

namespace StormTime.Weapon
{
    public class Bullet : KinematicBody2D
    {
        private static readonly PackedScene BulletExplosion =
            ResourceLoader.Load<PackedScene>(GameConstants.BulletExplosionPrefab);

        [Export] public float bulletSpeed;
        [Export] public float bulletLifeTime;

        private Vector2 _launchVelocity;
        private float _currentBulletTimeLeft;

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

        public void DestroyBullet()
        {
            GD.Print("Bullet Removed From World");
            GetParent().RemoveChild(this);
        }

        public void SpawnBulletExplosion()
        {
            Node2D bulletExplosionInstance = (Node2D)BulletExplosion.Instance();
            bulletExplosionInstance.SetPosition(GetGlobalPosition());
            GetParent().AddChild(bulletExplosionInstance);
        }
    }
}
