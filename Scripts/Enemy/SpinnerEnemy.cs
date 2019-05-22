using Godot;
using StormTime.Weapon;
using System;

namespace StormTime.Enemy
{
    public class SpinnerEnemy : Enemy
    {
        [Export] public float rotationRate;
        [Export] public float rotationSwitchTime;
        [Export] public PackedScene enemyBulletPrefab;
        [Export] public float timeBetweenShot;

        private bool _isForwardRotation;
        private float _currentRotationTime;
        private float _currentShootingTime;

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);
            RotateAndAttack(delta);
            CheckAndShoot(delta);
        }

        private void CheckAndShoot(float delta)
        {
            _currentShootingTime -= delta;

            if (_currentShootingTime <= 0)
            {
                _currentShootingTime = timeBetweenShot;
                ShootBullet();
            }
        }

        private void ShootBullet()
        {
            foreach (Node2D launchPoint in _launchPoints)
            {
                Bullet bulletInstance = (Bullet)enemyBulletPrefab.Instance();
                bulletInstance.SetPosition(launchPoint.GetGlobalPosition());
                bulletInstance.LaunchBullet(launchPoint.GetTransform().x);

                GetParent().AddChild(bulletInstance);
            }
        }

        private void RotateAndAttack(float delta)
        {
            _currentRotationTime -= delta;

            if (_currentRotationTime <= 0)
            {
                _isForwardRotation = !_isForwardRotation;
                _currentRotationTime = rotationSwitchTime;
            }

            if (_isForwardRotation)
            {
                Rotate(Mathf.Deg2Rad(rotationRate * delta));
            }
            else
            {
                Rotate(Mathf.Deg2Rad(-rotationRate * delta));
            }
        }
    }
}