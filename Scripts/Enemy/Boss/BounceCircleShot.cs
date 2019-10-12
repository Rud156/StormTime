using System;
using Godot;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class BounceCircleShot : BossBaseAttack
    {
        // Prefabs
        [Export] public PackedScene bulletPrefab;

        // Shot Info
        [Export] public NodePath spawnLeftMostPointNodePath;
        [Export] public NodePath spawnRightMostPointNodePath;

        // Launch Info
        [Export] public float timeBetweenRows;
        [Export] public int bulletsInEachRow;
        [Export] public Vector2 bulletDefaultVelocity;

        private Node2D _spawnLeftMostPoint;
        private Node2D _spawnRightMostPoint;

        private float _currentTimer;

        public override void _Ready()
        {
            _spawnLeftMostPoint = GetNode<Node2D>(spawnLeftMostPointNodePath);
            _spawnRightMostPoint = GetNode<Node2D>(spawnRightMostPointNodePath);
        }

        #region Overridden Parent

        public override bool UpdateAttack(float delta)
        {
            UpdateShot(delta);
            return base.UpdateAttack(delta);
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();
            _currentTimer = 0;
        }

        #endregion

        #region Utility Functions

        private void UpdateShot(float delta)
        {
            _currentTimer -= delta;
            if (_currentTimer <= 0)
            {
                LaunchBullets();
                _currentTimer = timeBetweenRows;
            }
        }

        private void LaunchBullets()
        {
            for (int i = 0; i < bulletsInEachRow; i++)
            {
                BossBullet bossBulletInstance = (BossBullet)bulletPrefab.Instance();
                _bulletHolder.AddChild(bossBulletInstance);

                float indexRatio = i / bulletsInEachRow;
                Vector2 finalPosition = _spawnLeftMostPoint.GetGlobalPosition().LinearInterpolate(_spawnRightMostPoint.GetGlobalPosition(), indexRatio);
                bossBulletInstance.SetGlobalPosition(finalPosition);

                bossBulletInstance.LaunchBullet(bulletDefaultVelocity.Normalized());
            }
        }

        #endregion
    }
}