using Godot;
using StormTime.Player.Data;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Enemy.Individuals
{
    public class BombEnemy : Enemy
    {
        [Export] public float timeBetweenShots;
        [Export] public PackedScene bombPrefab;
        [Export] public float rotationRate;

        private float _targetRotation;
        private float _currentAttackTimer;


        #region Overridden Parent

        protected override void LaunchAttack()
        {
            base.LaunchAttack();
            _currentAttackTimer = timeBetweenShots;
        }

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);

            if (_currentAttackTimer > 0)
            {
                OrientEnemyToTarget(delta, PlayerVariables.LastPlayerPosition);
                _currentAttackTimer -= delta;
            }
            else if (_currentAttackTimer <= 0)
            {
                EnemyLaunchSingleShotAttack();
                _currentAttackTimer = timeBetweenShots;
            }
        }

        protected override void EnemyLaunchSingleShotAttack()
        {
            BombBullet bombBulletInstance = (BombBullet)bombPrefab.Instance();
            GetParent().AddChild(bombBulletInstance);

            Node2D launchPoint = _launchPoints[0];
            bombBulletInstance.SetGlobalPosition(launchPoint.GetGlobalPosition());

            float rotation = _rotationNode.GetGlobalRotation();
            float xVelocity = Mathf.Cos(rotation);
            float yVelocity = Mathf.Sin(rotation);
            Vector2 launchVector = new Vector2(xVelocity, yVelocity);
            bombBulletInstance.LaunchBullet(launchVector.Normalized());
        }

        protected override void OrientEnemyToTarget(float delta, Vector2 targetPosition)
        {
            Vector2 currentPosition = GetGlobalPosition();
            _targetRotation = -Mathf.Rad2Deg(Mathf.Atan2(
                currentPosition.x - targetPosition.x,
                currentPosition.y - targetPosition.y
            )) - 90;


            float currentRotation = ExtensionFunctions.LerpAngleDeg(_rotationNode.GetGlobalRotationDegrees(), _targetRotation, rotationRate * delta);
            _rotationNode.SetGlobalRotationDegrees(currentRotation);
        }

        #endregion
    }
}