using Godot;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class FrenzySpinningShot : BossBaseAttack
    {
        // Display Affectors
        [Export] public float rotationOffset;

        // Shot Info
        [Export] public int totalCircleBullets;
        [Export] public float totalSprayBullets;
        [Export] public float sprayRotationIncrementAmount;
        [Export] public float timeBetweenSprayShots;
        [Export] public float timeBetweenCircleShots;

        private float _currentSprayTimer;
        private float _currentCircleTimer;
        private float _currentRotationAmount;

        #region Overridden Parent

        public override bool UpdateAttack(float delta)
        {
            UpdateShooting(delta);
            return base.UpdateAttack(delta);
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            _currentSprayTimer = 0;
            _currentCircleTimer = 0;
        }

        #endregion

        #region Utility Functions

        private void UpdateShooting(float delta)
        {
            _currentSprayTimer -= delta;
            if (_currentSprayTimer <= 0)
            {
                LaunchSprayShot();
                _currentSprayTimer = timeBetweenSprayShots;
            }

            _currentCircleTimer -= delta;
            if (_currentCircleTimer <= 0)
            {
                LaunchCircleShot();

                _currentCircleTimer = timeBetweenCircleShots;
                _currentRotationAmount += sprayRotationIncrementAmount;
            }
        }

        private void LaunchCircleShot()
        {
            float angleDiff = 360.0f / totalCircleBullets;
            float currentCircleAngle = 0;

            for (int i = 0; i < totalCircleBullets; i++)
            {
                BossBullet bossBulletInstance = (BossBullet)bulletPrefab.Instance();
                _bulletHolder.AddChild(bossBulletInstance);

                bossBulletInstance.SetGlobalRotationDegrees(currentCircleAngle + rotationOffset);

                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(currentCircleAngle));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(currentCircleAngle));
                Vector2 launchVector = new Vector2(xVelocity, yVelocity);
                bossBulletInstance.LaunchBullet(launchVector.Normalized());

                currentCircleAngle += angleDiff;
            }
        }

        private void LaunchSprayShot()
        {
            float angleDiff = 360.0f / totalSprayBullets;
            float currentSprayAngle = _currentRotationAmount;

            for (int i = 0; i < totalSprayBullets; i++)
            {
                BossBullet bossBulletInstance = (BossBullet)bulletPrefab.Instance();
                _bulletHolder.AddChild(bossBulletInstance);

                bossBulletInstance.SetGlobalRotationDegrees(currentSprayAngle + rotationOffset);

                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(currentSprayAngle));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(currentSprayAngle));
                Vector2 launchVector = new Vector2(xVelocity, yVelocity);
                bossBulletInstance.LaunchBullet(launchVector);

                currentSprayAngle += angleDiff;
            }
        }

        #endregion
    }
}