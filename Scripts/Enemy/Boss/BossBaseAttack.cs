using Godot;

namespace StormTime.Enemy.Boss
{
    public abstract class BossBaseAttack : Node
    {
        [Export] public float attackTimer;

        private bool _attackLaunched = false;
        private float _currentAttackTimer = -1;

        public virtual bool UpdateAttack(float delta)
        {
            if (!_attackLaunched)
            {
                LaunchAttack();
                _attackLaunched = true;
            }

            _currentAttackTimer -= delta;

            if (_currentAttackTimer <= 0)
            {
                _attackLaunched = false;
                return true;
            }

            return false;
        }

        public virtual void LaunchAttack() => _currentAttackTimer = attackTimer;
    }
}