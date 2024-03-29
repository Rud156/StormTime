using Godot;
using StormTime.Weapon;

namespace StormTime.Enemy.Individuals
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
                EnemyLaunchSingleShotAttack();
            }
        }

        protected override void EnemyLaunchSingleShotAttack()
        {
            float startRotation = 0;
            float rotationIncrementAmount = (float)360 / _launchPoints.Count;

            foreach (Node2D launchPoint in _launchPoints)
            {
                EnemyBullet bulletInstance = (EnemyBullet)enemyBulletPrefab.Instance();
                GetParent().AddChild(bulletInstance);

                bulletInstance.SetBulletColor(_bulletColor);
                bulletInstance.SetGlobalPosition(launchPoint.GetGlobalPosition());

                float rotation = _rotationNode.GetRotationDegrees() + startRotation - 90;
                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(rotation));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(rotation));
                Vector2 launchVector = new Vector2(xVelocity, yVelocity);
                bulletInstance.LaunchBullet(launchVector.Normalized());

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

            _rotationNode.Rotate(_isForwardRotation
                ? Mathf.Deg2Rad(rotationRate * delta)
                : Mathf.Deg2Rad(-rotationRate * delta));
        }
    }
}