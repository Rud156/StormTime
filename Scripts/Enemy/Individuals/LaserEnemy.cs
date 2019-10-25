using Godot;
using StormTime.Player.Data;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Enemy.Individuals
{
    public class LaserEnemy : Enemy
    {
        // Shooting
        [Export] public float timeBetweenLaserShot;
        [Export] public float timeBeforeLaserShot;
        [Export] public float laserShotTotalTime;

        // Prefabs
        [Export] public PackedScene laserPrefab;
        [Export] public PackedScene laserBulletPrefab;

        // Movement
        [Export] public float rotationRate;
        [Export] public float rotationOffset;

        // Effects
        [Export] public NodePath laserLaunchEffectNodePath;

        private float _currentTimeBetweenLaserShot;
        private float _currentTimeLeftBeforeLaserShot;
        private float _currentTimeLeftForLaserShot;

        private Particles2D _laserLaunchEffect;
        private EnemyLaser _currentLaser;
        private bool _laserLaunched;

        private float _targetRotation;

        public override void _Ready()
        {
            base._Ready();

            _laserLaunchEffect = GetNode<Particles2D>(laserLaunchEffectNodePath);
        }

        #region Overridden Parent

        protected override void LaunchAttack()
        {
            base.LaunchAttack();
            ResetTimers();
        }

        protected override void EndAttack()
        {
            base.EndAttack();

            _currentLaser?.DestroyLaser();
            _currentLaser = null;

            _laserLaunchEffect.SetEmitting(false);
        }

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);

            if (_currentTimeBetweenLaserShot > 0)
            {
                _currentTimeBetweenLaserShot -= delta;
                OrientEnemyToPlayer(delta);
            }
            else if (_currentTimeBetweenLaserShot <= 0)
            {
                if (!_laserLaunchEffect.IsEmitting())
                {
                    _laserLaunchEffect.SetEmitting(true);
                }
                _currentTimeLeftBeforeLaserShot -= delta;

                if (_currentTimeLeftBeforeLaserShot <= 0)
                {
                    CheckAndLaunchLaserAttack();
                    _currentTimeLeftForLaserShot -= delta;

                    if (_currentTimeLeftForLaserShot <= 0)
                    {
                        _currentLaser.DestroyLaser();
                        _currentLaser = null;

                        _laserLaunchEffect.SetEmitting(false);
                        ResetTimers();
                    }
                }
            }
        }

        protected override void EnemyLaunchSingleShotAttack()
        {
            EnemyBullet bulletInstance = (EnemyBullet)laserBulletPrefab.Instance();
            GetParent().AddChild(bulletInstance);

            Node2D launchPoint = _launchPoints[0];
            bulletInstance.SetGlobalPosition(launchPoint.GetGlobalPosition());

            float rotation = _rotationNode.GetGlobalRotation();
            float xVelocity = Mathf.Cos(rotation);
            float yVelocity = Mathf.Sin(rotation);
            Vector2 launchVector = new Vector2(xVelocity, yVelocity);
            bulletInstance.LaunchBullet(launchVector.Normalized());
        }

        protected override void OrientEnemyToPlayer(float delta)
        {
            Vector2 currentPosition = GetGlobalPosition();
            _targetRotation = -Mathf.Rad2Deg(Mathf.Atan2(
                currentPosition.x - PlayerVariables.LastPlayerPosition.x,
                currentPosition.y - PlayerVariables.LastPlayerPosition.y
            )) - 90;


            float currentRotation = ExtensionFunctions.LerpAngleDeg(GetGlobalRotationDegrees(), _targetRotation, rotationRate * delta);
            SetGlobalRotation(currentRotation);
        }

        #endregion

        #region Utility Functions

        private void CheckAndLaunchLaserAttack()
        {
            if (_laserLaunched)
            {
                return;
            }

            EnemyLaser enemyLaserInstance = (EnemyLaser)laserPrefab.Instance();
            GetParent().AddChild(enemyLaserInstance);

            Node2D launchPoint = _launchPoints[0];
            enemyLaserInstance.SetGlobalPosition(launchPoint.GetGlobalPosition());
            enemyLaserInstance.SetGlobalRotationDegrees(_rotationNode.GetGlobalRotationDegrees() + rotationOffset);
            enemyLaserInstance.LaunchBullet(Vector2.Zero);

            _currentLaser = enemyLaserInstance;
            _laserLaunched = true;
        }

        private void ResetTimers()
        {
            _currentTimeBetweenLaserShot = timeBetweenLaserShot;
            _currentTimeLeftBeforeLaserShot = timeBeforeLaserShot;
            _currentTimeLeftForLaserShot = laserShotTotalTime;
        }

        #endregion
    }
}