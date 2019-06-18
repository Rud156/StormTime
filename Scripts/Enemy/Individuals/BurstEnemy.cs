using Godot;
using StormTime.Weapon;

namespace StormTime.Enemy.Individuals
{
    public class BurstEnemy : Enemy
    {
        // Rotation Attack Stage
        [Export] public float rotationRate;
        [Export] public float rotationTime;

        // Attack Stage
        [Export] public float rotationAttackCounts;
        [Export] public float timeDelayBetweenShots;

        // Bullet
        [Export] public PackedScene enemyBulletPrefab;

        private enum EnemyAttackState
        {
            AttackRotating,
            AttackAttacking
        }

        private float _enemyAttackTimer;
        private int _rotationAttackCounter;
        private int _forwardRotationDirection;
        private float _currentRotationRate;
        private EnemyAttackState _enemyAttackState;

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);

            switch (_enemyAttackState)
            {
                case EnemyAttackState.AttackRotating:
                    UpdateAttackRotatingState(delta);
                    break;

                case EnemyAttackState.AttackAttacking:
                    UpdateAttackAttackingState(delta);
                    break;
            }
        }

        protected override void LaunchAttack()
        {
            base.LaunchAttack();
            ResetAndStartRotating();
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            ResetAndStartRotating();
        }

        #region Enemy Attack State Updates

        private void UpdateAttackRotatingState(float delta)
        {
            _enemyAttackTimer -= delta;

            if (_enemyAttackTimer <= 0)
            {
                _enemyAttackTimer = 0;
                SetEnemyAttackState(EnemyAttackState.AttackAttacking);
            }

            _rotationNode.Rotate(Mathf.Deg2Rad(_currentRotationRate * delta * _forwardRotationDirection));
        }

        private void UpdateAttackAttackingState(float delta)
        {
            _enemyAttackTimer -= delta;
            if (_enemyAttackTimer <= 0)
            {
                ShootBullets();
                _enemyAttackTimer = timeDelayBetweenShots;
                _rotationAttackCounter += 1;
            }

            if (_rotationAttackCounter > rotationAttackCounts)
            {
                ResetAndStartRotating();
            }
        }

        private void ResetAndStartRotating()
        {
            _enemyAttackTimer = rotationTime;
            _rotationAttackCounter = 0;
            _forwardRotationDirection = GD.Randf() <= 0.5f ? -1 : 1;
            _currentRotationRate = (float)GD.RandRange(rotationRate, 2 * rotationTime);
            SetEnemyAttackState(EnemyAttackState.AttackRotating);
        }

        private void ShootBullets()
        {
            float startRotation = 0;
            float rotationIncrementAmount = 360.0f / _launchPoints.Count;

            foreach (Node2D launchPoint in _launchPoints)
            {
                EnemyBullet bulletInstance = (EnemyBullet)enemyBulletPrefab.Instance();
                GetParent().AddChild(bulletInstance);

                bulletInstance.SetBulletColor(_bulletColor);
                bulletInstance.SetGlobalPosition(launchPoint.GetGlobalPosition());

                float rotation = _rotationNode.GetRotationDegrees() + startRotation;
                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(rotation));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(rotation));
                Vector2 launchVector = new Vector2(xVelocity, yVelocity);
                bulletInstance.LaunchBullet(launchVector.Normalized());

                startRotation += rotationIncrementAmount;
            }
        }

        #endregion

        #region Utility Functions

        private void SetEnemyAttackState(EnemyAttackState attackState)
        {
            if (attackState == _enemyAttackState)
            {
                return;
            }

            _enemyAttackState = attackState;
        }

        #endregion
    }
}