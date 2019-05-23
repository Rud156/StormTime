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
        [Export] public Color bulletColor;

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
                ShootBullets();
            }
        }

        private void ShootBullets()
        {
            float startRotation = 0;
            float rotationIncrementAmount = 360 / _launchPoints.Count;

            foreach (Node2D launchPoint in _launchPoints)
            {
                EnemyBullet bulletInstance = (EnemyBullet)enemyBulletPrefab.Instance();
                bulletInstance.SetPosition(launchPoint.GetGlobalPosition());
                bulletInstance.SetBulletColor(bulletColor);

                float rotation = GetRotationDegrees() + startRotation - 90;
                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(rotation));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(rotation));
                Vector2 launchVector = new Vector2(xVelocity, yVelocity);
                bulletInstance.LaunchBullet(launchVector.Normalized());

                GetParent().AddChild(bulletInstance);

                startRotation += rotationIncrementAmount;
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