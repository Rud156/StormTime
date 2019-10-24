using Godot;

namespace StormTime.Enemy.Individuals
{
    public class BombEnemy : Enemy
    {
        [Export] public float shootWaitDelay;
        [Export] public PackedScene bombPrefab;
        [Export] public float playerTargetRotationRate;

        #region Overridden Parent

        protected override void LaunchAttack()
        {

        }

        protected override void EndAttack()
        {

        }

        protected override void UpdateAttacking(float delta)
        {

        }

        protected override void EnemyLaunchSingleShotAttack()
        {

        }

        #endregion
    }
}