using Godot;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class InnerCircleShot : BossBaseAttack
    {
        // Shot Info
        [Export] public float angleIncrementAmount;
        [Export] public float timeBetweenShot;

        private float _currentAngle;
        private float _angleTimer;


        #region Overridden Parent

        public override bool UpdateAttack(float delta)
        {
            bool attackComplete = base.UpdateAttack(delta);
            UpdateRotationAndShoot(delta);
            return attackComplete;
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            _angleTimer = 0;
        }

        #endregion

        #region Utility Functions

        private void UpdateRotationAndShoot(float delta)
        {
            _angleTimer -= delta;
            if (_angleTimer <= 0)
            {
                LaunchBullet();

                _currentAngle += angleIncrementAmount;
                _angleTimer = timeBetweenShot;
            }
        }

        private void LaunchBullet()
        {
            BossBullet bossBulletInstance = (BossBullet)bulletPrefab.Instance();
            _bulletHolder.AddChild(bossBulletInstance);

            bossBulletInstance.SetGlobalPosition(_bossAttackPoint.GetGlobalPosition());

            float xVelocity = Mathf.Cos(Mathf.Deg2Rad(_currentAngle));
            float yVelocity = Mathf.Sin(Mathf.Deg2Rad(_currentAngle));
            Vector2 launchVector = new Vector2(xVelocity, yVelocity);
            bossBulletInstance.LaunchBullet(launchVector.Normalized());
        }

        #endregion
    }
}