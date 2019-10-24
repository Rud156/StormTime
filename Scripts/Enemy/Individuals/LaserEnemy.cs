using Godot;
using StormTime.Weapon;

namespace StormTime.Enemy.Individuals
{
    public class LaserEnemy : Enemy
    {
        // Shooting
        [Export] public float shootWaitDelay;
        [Export] public float shootTimeLength;

        // Prefabs
        [Export] public PackedScene laserPrefab;

        // Movement
        [Export] public float playerTargetRotationRate;
        [Export] public float rotationOffset;

        private float _shootWaitTimer;
        private float _shootTimer;

        private EnemyLaser _currentLaser;

        #region Overridden Parent

        protected override void LaunchAttack()
        {
            _shootWaitTimer = shootWaitDelay;
            _shootTimer = shootTimeLength;
        }

        protected override void EndAttack()
        {

        }

        protected override void UpdateAttacking(float delta)
        {
            _shootWaitTimer -= delta;
            if (_shootWaitTimer <= 0)
            {
                EnemyLaser enemyLaser = (EnemyLaser)laserPrefab.Instance();
                GetParent().AddChild(enemyLaser);

                _currentLaser = enemyLaser;

                
            }
        }

        protected override void EnemyLaunchSingleShotAttack()
        {

        }

        #endregion
    }
}