using Godot;
using StormTime.Player.Data;

namespace StormTime.Enemy.Individuals
{
    public class SlasherEnemy : Enemy
    {
        // Player Targeting and Attack
        [Export] public float attackingPlayerMovementSpeed;
        [Export] public float attackingWaitTime;
        [Export] public float rotationRate;
        [Export] public NodePath slasherEffectNode;
        [Export] public NodePath slasherEnemyCollisionNodePath;

        private Particles2D _slasherEffect;
        private float _currentRotationRate;

        private SlasherEnemyCollision _slasherEnemyCollision;

        public override void _Ready()
        {
            base._Ready();

            _slasherEffect = GetNode<Particles2D>(slasherEffectNode);
            _slasherEnemyCollision = GetNode<SlasherEnemyCollision>(slasherEnemyCollisionNodePath);
        }

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);

            if (_enemyTimer > attackingWaitTime)
            {
                _targetPosition = PlayerVariables.LastPlayerPosition;
                MoveToTowardsTarget(_targetPosition, attackingPlayerMovementSpeed);
            }
            else
            {
                _currentRotationRate -= delta * attackingWaitTime * rotationRate;
                _currentRotationRate = _currentRotationRate <= 0 ? 0 : _currentRotationRate;
                _slasherEffect.SetEmitting(false);
            }

            _rotationNode.Rotate(Mathf.Deg2Rad(_currentRotationRate * delta));
        }

        protected override void LaunchAttack()
        {
            base.LaunchAttack();

            _currentRotationRate = rotationRate;
            _slasherEffect.SetEmitting(true);

            _slasherEnemyCollision.SetAttackingState(true);
        }

        protected override void EndAttack()
        {
            base.EndAttack();

            _slasherEffect.SetEmitting(false);
            _slasherEnemyCollision.SetAttackingState(false);
        }
    }
}