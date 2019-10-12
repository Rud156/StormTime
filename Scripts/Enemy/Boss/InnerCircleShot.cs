using Godot;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class InnerCircleShot : BossBaseAttack
    {
        // Prefab
        [Export] public PackedScene bulletPrefab;

        // Shot Info
        [Export] public float angleIncrementAmount;
        [Export] public float angleIncrementRate;

        private float _currentAngle;
        private float _angleTimer;


        #region Overridden Parent

        public override bool UpdateAttack(float delta)
        {
            UpdateRotationAndShoot(delta);

            return base.UpdateAttack(delta);
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            _currentAngle = 0;
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
                _angleTimer = angleIncrementRate;
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