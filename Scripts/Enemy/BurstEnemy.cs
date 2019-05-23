using Godot;
using System;

namespace StormTime.Enemy
{
    public class BurstEnemy : Enemy
    {
        // Rotation Attack Stage
        [Export] public float rotationRate;
        [Export] public float rotationTime;

        // Attack Stage
        [Export] public float rotationAttackCounts;
        [Export] public float timeDelayBetweenShots;

        private enum EnemyAttackState
        {
            AttackRotating,
            AttackAttacking
        }

        private float _enemyAttackTimer;
        private int _rotationAttackCounter;
        private int _forwardRotationDirection;
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

            Rotate(Mathf.Deg2Rad(rotationRate * delta * _forwardRotationDirection));
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
            SetEnemyAttackState(EnemyAttackState.AttackRotating);
        }

        private void ShootBullets()
        {

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