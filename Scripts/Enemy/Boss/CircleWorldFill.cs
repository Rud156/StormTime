using Godot;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class CircleWorldFill : BossBaseAttack
    {
        // Shot Info
        [Export] public float eachShotBulletCount;
        [Export] public float timeBetweenShot;
        [Export] public float angleIncrementAmount;

        private float _currentAngle;
        private float _currentTime;

        #region Overridden Parent

        public override bool UpdateAttack(float delta)
        {
            UpdateShooting(delta);
            return base.UpdateAttack(delta);
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            _currentTime = 0;
        }

        #endregion

        #region Utility Functions

        private void UpdateShooting(float delta)
        {
            _currentTime -= delta;
            if (_currentTime <= 0)
            {
                LaunchBullet();

                _currentTime = timeBetweenShot;
                _currentAngle += angleIncrementAmount;
            }
        }

        private void LaunchBullet()
        {
            float currentShotAngle = _currentAngle;
            float angleDiff = 360.0f / eachShotBulletCount;

            for (int i = 0; i < eachShotBulletCount; i++)
            {
                BossBullet bossBulletInstance = (BossBullet)bulletPrefab.Instance();
                _bulletHolder.AddChild(bossBulletInstance);

                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(currentShotAngle));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(currentShotAngle));
                Vector2 launchVector = new Vector2(xVelocity, yVelocity);
                bossBulletInstance.LaunchBullet(launchVector.Normalized());

                currentShotAngle += angleDiff;
                currentShotAngle = ExtensionFunctions.To360Angle(currentShotAngle);
            }
        }

        #endregion
    }
}