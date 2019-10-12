using Godot;

namespace StormTime.Enemy.Boss
{
    public abstract class BossBaseAttack : Node
    {
        [Export] public PackedScene bulletPrefab;
        [Export] public float attackTimer;
        [Export] public NodePath bulletHolderNodePath;
        [Export] public NodePath bossAttackPointNodePath;

        private bool _attackLaunched = false;
        private float _currentAttackTimer = -1;

        protected Node2D _bulletHolder;
        protected Node2D _bossAttackPoint;

        public override void _Ready()
        {
            _bulletHolder = GetNode<Node2D>(bulletHolderNodePath);
            _bossAttackPoint = GetNode<Node2D>(bossAttackPointNodePath);
        }

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